using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Servicies;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AuraDecor.Core.Entities.Enums;

namespace AuraDecor.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        private readonly IConfiguration _configuration;

        public OrderController(
            IOrderService orderService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OrderController> logger,
            IConfiguration configuration)
        {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<OrderCreationResponseDto>> CreateOrderAsync([FromBody] CreateOrderDto createOrderDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse(401, "User not authenticated"));
            }

            
            var shippingAddress = _mapper.Map<AuraDecor.Core.Entities.Address>(createOrderDto.ShippingAddress);
            
            var (order, paymentIntent) = await _orderService.CreateOrderAsync(
                userId, 
                createOrderDto.CartId, 
                shippingAddress);
            
            var orderToReturn = _mapper.Map<OrderToReturnDto>(order);
            
            var paymentIntentDto = new PaymentIntentDto
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.PaymentIntentId,
                PublishableKey = _configuration["Stripe:PublishableKey"],
                Success = true,
                Message = "Payment intent created successfully"
            };
            
            
            var response = new OrderCreationResponseDto
            {
                Order = orderToReturn,
                PaymentIntent = paymentIntentDto
            };
            
            return Ok(response);
        }
        
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdAsync(Guid orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse(401, "User not authenticated"));
            }
            
            _logger.LogInformation($"Getting order {orderId} for user {userId}");
            
            var order = await _orderService.GetOrderByIdAsync(orderId, userId);
            
            if (order == null)
            {
                _logger.LogWarning($"Order {orderId} not found for user {userId}");
                return NotFound(new ApiResponse(404, "Order not found"));
            }
            
            var orderToReturn = _mapper.Map<OrderToReturnDto>(order);
            return Ok(orderToReturn);
        }
        
        [HttpGet("user-orders")]
        public async Task<ActionResult<IEnumerable<OrderToReturnDto>>> GetUserOrdersAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse(401, "User not authenticated"));
            }
            
            _logger.LogInformation($"Getting all orders for user {userId}");
            
            var orders = await _orderService.GetOrdersForUserAsync(userId);
            
            var ordersToReturn = _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
            return Ok(ordersToReturn);
        }
        
        [HttpPost("cancel")]
        public async Task<ActionResult<bool>> CancelOrderAsync([FromBody] Guid orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse(401, "User not authenticated"));
            }

            _logger.LogInformation($"User {userId} attempting to cancel order {orderId}");
            var result = await _orderService.CancelOrderAsync(userId, orderId);
            
            if (!result)
            {
                _logger.LogWarning($"Failed to cancel order {orderId} for user {userId}");
                return BadRequest(new ApiResponse(400, "Could not cancel order. The order may be already paid or not found."));
            }

            _logger.LogInformation($"Order {orderId} successfully cancelled by user {userId}");
            return Ok(new { cancelled = true, message = "Order cancelled successfully" });
        }
        

        

        


        [HttpPut("update-status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderToReturnDto>> UpdateOrderStatus([FromBody] UpdateOrderStatusDto updateDto)
        {
            _logger.LogInformation($"Attempting to update order {updateDto.OrderId} status to {updateDto.Status}");
            
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(updateDto.OrderId);
            if (order == null)
            {
                _logger.LogWarning($"Order {updateDto.OrderId} not found");
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            if (!Enum.TryParse<OrderStatus>(updateDto.Status, true, out var newStatus))
            {
                _logger.LogWarning($"Invalid order status: {updateDto.Status}");
                return BadRequest(new ApiResponse(400, $"Invalid order status: {updateDto.Status}"));
            }
            
            if (order.Status == OrderStatus.Cancelled)
            {
                _logger.LogWarning($"Cannot update status of cancelled order {updateDto.OrderId}");
                return BadRequest(new ApiResponse(400, "Cannot update status of cancelled order"));
            }
            
            order.Status = newStatus;
            await _unitOfWork.CompleteAsync();
            
            _logger.LogInformation($"Order {updateDto.OrderId} status updated to {newStatus}");
            
            var orderToReturn = _mapper.Map<OrderToReturnDto>(order);
            return Ok(orderToReturn);
        }


    }
}
