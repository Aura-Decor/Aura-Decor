using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications.OrderSpecification;
using AuraDecor.Servicies;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace AuraDecor.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public OrderController(IOrderService orderService , IMapper mapper , IPaymentService paymentService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _paymentService = paymentService;
        }

        [HttpPost("CreatOrder")]

        public async Task<ActionResult<Order>> CreateOrderAsync(string UserId, Guid CartId)
        {
            var order = await _orderService.CreateOrderAsync(UserId, CartId);

            return Ok(order);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<Order>> GetOrderByUserIdAysnc(string Id)
        {
            var order = await _orderService.GetOrderByUserIdAsync(Id);
            return Ok(order);

        }
        [HttpPost("CancelOrder")]
        public async Task<ActionResult<bool>> CancelOrderAsync(string UserId, Guid OrderId)
        {
            var order = await _orderService.GetOrderByUserIdAsync(UserId);
            if(order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));

            }
            await _orderService.CancelOrderAsync(UserId, OrderId);
            return Ok(true);

        }

        [HttpPost("payment-intent")]
        public async Task<ActionResult<PaymentIntent>> CreateOrUpdatePaymentIntent(Guid cartId)
        {
            try
            {
                var intent = await _paymentService.CreateOrUpdatePaymentIntentAysnc(cartId);
                return Ok(intent);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }

    }
}
