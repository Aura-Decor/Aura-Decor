using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications.OrderSpecification;
using AuraDecor.Servicies;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        private readonly IConfiguration _configuration;

        public OrderController(
            IOrderService orderService,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OrderController> logger,
            IConfiguration configuration)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<OrderCreationResponseDto>> CreateOrderAsync([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse(401, "User not authenticated"));
                }

                _logger.LogInformation($"Creating order for cart {createOrderDto.CartId}, user {userId}");
                
                var shippingAddress = _mapper.Map<AuraDecor.Core.Entities.Address>(createOrderDto.ShippingAddress);
                
                var order = await _orderService.CreateOrderAsync(
                    userId, 
                    createOrderDto.CartId, 
                    shippingAddress);
                
                var orderToReturn = _mapper.Map<OrderToReturnDto>(order);
                
                PaymentIntentDto paymentIntentDto = null;
                try
                {
                    var paymentIntent = await _paymentService.GetPaymentIntentClientSecret(createOrderDto.CartId);
                    paymentIntentDto = new PaymentIntentDto
                    {
                        ClientSecret = paymentIntent.ClientSecret,
                        PaymentIntentId = paymentIntent.PaymentIntentId,
                        PublishableKey = _configuration["Stripe:PublishableKey"],
                        Success = true,
                        Message = "Payment intent created successfully"
                    };
                    
                    order.PaymentIntentId = paymentIntent.PaymentIntentId;
                    await _unitOfWork.CompleteAsync();
                    
                    orderToReturn = _mapper.Map<OrderToReturnDto>(order);
                    
                    _logger.LogInformation($"Payment intent created for order {order.Id}: {paymentIntentDto.PaymentIntentId}");
                }
                catch (Exception paymentEx)
                {
                    _logger.LogError(paymentEx, $"Failed to create payment intent for order {order.Id}");
                }
                
                var response = new OrderCreationResponseDto
                {
                    Order = orderToReturn,
                    PaymentIntent = paymentIntentDto
                };
                
                _logger.LogInformation($"Order created successfully: {order.Id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }
        
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdAsync(Guid orderId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order {orderId}");
                return StatusCode(500, new ApiResponse(500, ex.Message));
            }
        }
        
        [HttpGet("user-orders")]
        public async Task<ActionResult<IEnumerable<OrderToReturnDto>>> GetUserOrdersAsync()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user orders");
                return StatusCode(500, new ApiResponse(500, ex.Message));
            }
        }
        
        [HttpPost("cancel")]
        public async Task<ActionResult<bool>> CancelOrderAsync([FromBody] Guid orderId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {orderId}");
                return StatusCode(500, new ApiResponse(500, ex.Message));
            }
        }
        
        [HttpGet("payment-intent/{cartId}")]
        public async Task<ActionResult<PaymentIntentDto>> GetPaymentIntent(Guid cartId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse(401, "User not authenticated"));
                }
                
                _logger.LogInformation($"Getting payment intent for cart {cartId}, user {userId}");

                // Check if the cart exists and belongs to the user
                var cart = await _unitOfWork.Repository<Cart>().GetByIdAsync(cartId);
                if (cart == null)
                {
                    _logger.LogWarning($"Cart {cartId} not found");
                    return NotFound(new ApiResponse(404, "Cart not found"));
                }
                
                if (cart.UserId != userId)
                {
                    _logger.LogWarning($"User {userId} attempted to access cart {cartId} owned by {cart.UserId}");
                    return Unauthorized(new ApiResponse(401, "Unauthorized access to cart"));
                }
                
                // Check that the cart has items
                if (cart.CartItems == null || !cart.CartItems.Any())
                {
                    _logger.LogWarning($"Cart {cartId} is empty");
                    return BadRequest(new ApiResponse(400, "Cannot create payment intent for empty cart"));
                }

                var paymentIntent = await _paymentService.GetPaymentIntentClientSecret(cartId);
                var paymentIntentDto = new Dtos.Outgoing.PaymentIntentDto
                {
                    ClientSecret = paymentIntent.ClientSecret,
                    PaymentIntentId = paymentIntent.PaymentIntentId,
                    PublishableKey = _configuration["Stripe:PublishableKey"],
                    Success = true,
                    Message = "Payment intent created successfully"
                };
                
                _logger.LogInformation($"Payment intent retrieved successfully: {paymentIntentDto.PaymentIntentId}");
                return Ok(paymentIntentDto);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"Stripe error retrieving payment intent for cart {cartId}: {ex.Message}");
                return BadRequest(new ApiResponse(400, $"Stripe error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payment intent for cart {cartId}");
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }
        
        [HttpGet("verify-payment/{paymentIntentId}")]
        public async Task<ActionResult<string>> VerifyPaymentStatus(string paymentIntentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse(401, "User not authenticated"));
                }
                
                if (string.IsNullOrEmpty(paymentIntentId))
                {
                    return BadRequest(new ApiResponse(400, "Payment intent ID is required"));
                }
                
                _logger.LogInformation($"Verifying payment status for intent {paymentIntentId}");
                
                var paymentStatus = await _paymentService.VerifyPaymentStatus(paymentIntentId);
                
                if (paymentStatus == PaymentStatus.Succeeded)
                {
                    await _paymentService.UpdateOrderPaymentSucceeded(paymentIntentId);
                }
                else if (paymentStatus == PaymentStatus.Failed)
                {
                    await _paymentService.UpdateOrderPaymentFailed(paymentIntentId);
                }
                
                return Ok(new { status = paymentStatus.ToString() });
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"Stripe error verifying payment status for intent {paymentIntentId}: {ex.Message}");
                return BadRequest(new ApiResponse(400, $"Stripe error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying payment status for intent {paymentIntentId}");
                return StatusCode(500, new ApiResponse(500, $"An error occurred: {ex.Message}"));
            }
        }
        
        [HttpGet("payment-order-details/{paymentIntentId}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByPaymentIntent(string paymentIntentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse(401, "User not authenticated"));
                }
                
                if (string.IsNullOrEmpty(paymentIntentId))
                {
                    return BadRequest(new ApiResponse(400, "Payment intent ID is required"));
                }
                
                _logger.LogInformation($"Getting order details for payment intent {paymentIntentId}");
                
                var paymentStatus = await _paymentService.VerifyPaymentStatus(paymentIntentId);
                
                var order = await _unitOfWork.Repository<Order>()
                    .FindAsync(o => o.PaymentIntentId == paymentIntentId);
                
                if (order == null)
                {
                    _logger.LogWarning($"Order not found for payment intent {paymentIntentId}");
                    return NotFound(new ApiResponse(404, "Order not found for this payment"));
                }
                
                if (order.UserId != userId && !User.IsInRole("Admin"))
                {
                    _logger.LogWarning($"User {userId} attempted to access order for user {order.UserId}");
                    return Unauthorized(new ApiResponse(401, "You don't have permission to view this order"));
                }
                
                var orderSpec = new OrdersWithSpecification(order.Id);
                var orderWithDetails = await _unitOfWork.Repository<Order>().GetWithSpecAsync(orderSpec);
                
                if (orderWithDetails == null)
                {
                    return NotFound(new ApiResponse(404, "Order details not found"));
                }
                
                if (paymentStatus == PaymentStatus.Succeeded && order.PaymentStatus != PaymentStatus.Succeeded)
                {
                    await _paymentService.UpdateOrderPaymentSucceeded(paymentIntentId);
                    
                    orderWithDetails = await _unitOfWork.Repository<Order>().GetWithSpecAsync(orderSpec);
                }
                
                var orderToReturn = _mapper.Map<OrderToReturnDto>(orderWithDetails);
                return Ok(new { 
                    order = orderToReturn, 
                    paymentStatus = paymentStatus.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order for payment intent {paymentIntentId}");
                return StatusCode(500, new ApiResponse(500, ex.Message));
            }
        }
        
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string webhookSecret = _configuration["Stripe:WebhookSecret"];
            
            if (string.IsNullOrEmpty(webhookSecret))
            {
                _logger.LogError("Stripe webhook secret is not configured");
                return StatusCode(500, new { Error = "Webhook processing failed - server misconfiguration" });
            }
            
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );
                
                _logger.LogInformation($"Stripe webhook received: {stripeEvent.Type} - {stripeEvent.Id}");
                
                await _unitOfWork.BeginTransactionAsync();
                
                try
                {
                    switch (stripeEvent.Type)
                    {
                        case "payment_intent.succeeded":
                            await HandlePaymentIntentSucceeded((PaymentIntent)stripeEvent.Data.Object);
                            break;
                            
                        case "payment_intent.payment_failed":
                            await HandlePaymentIntentFailed((PaymentIntent)stripeEvent.Data.Object);
                            break;
                            
                        case "payment_intent.canceled":
                            await HandlePaymentIntentCanceled((PaymentIntent)stripeEvent.Data.Object);
                            break;
                        
                        case "charge.refunded":
                            await HandleChargeRefunded((Charge)stripeEvent.Data.Object);
                            break;
                            
                        case "charge.dispute.created":
                            await HandleChargeDisputeCreated((Dispute)stripeEvent.Data.Object);
                            break;
                            
                        case "charge.dispute.resolved":
                            await HandleChargeDisputeResolved((Dispute)stripeEvent.Data.Object);
                            break;
                        
                        case "payment_method.attached":
                            var paymentMethod = (PaymentMethod)stripeEvent.Data.Object;
                            _logger.LogInformation($"Payment method attached: {paymentMethod.Id} for customer {paymentMethod.CustomerId}");
                            break;
                            
                        default:
                            _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
                            break;
                    }
                    
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, $"Error processing Stripe webhook event {stripeEvent.Type}: {ex.Message}");
                    
                }
                
                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook error: Invalid signature or malformed payload");
                
                return BadRequest(new { Error = "Invalid webhook signature" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return StatusCode(500, new { Error = "Webhook processing failed" });
            }
        }
        
        private async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
        {
            _logger.LogInformation($"Payment succeeded for intent: {paymentIntent.Id}");
            
            var metadata = paymentIntent.Metadata;
            string cartId = metadata != null && metadata.ContainsKey("CartId") ? metadata["CartId"] : null;
            string userId = metadata != null && metadata.ContainsKey("UserId") ? metadata["UserId"] : null;
            
            if (!string.IsNullOrEmpty(cartId) && !string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation($"Payment for Cart {cartId}, User {userId}");
            }
            
            var successResult = await _paymentService.UpdateOrderPaymentSucceeded(paymentIntent.Id);
            
            if (successResult)
            {
                _logger.LogInformation($"Order updated successfully for payment intent: {paymentIntent.Id}");
                
                var order = await _unitOfWork.Repository<Order>()
                    .FindAsync(o => o.PaymentIntentId == paymentIntent.Id);
                    
                if (order != null) 
                {
                    _logger.LogInformation($"Order {order.Id} updated to status {order.Status} for user {order.UserId}");
                    
                    // TODO: Send order confirmation email to customer
                    // TODO: Update inventory for purchased items
                }
            }
            else
            {
                _logger.LogWarning($"Failed to update order for payment intent: {paymentIntent.Id}");
            }
        }
        
        private async Task HandlePaymentIntentFailed(PaymentIntent paymentIntent)
        {
            _logger.LogWarning($"Payment failed for intent: {paymentIntent.Id}");
            
            var lastPaymentError = paymentIntent.LastPaymentError;
            if (lastPaymentError != null)
            {
                _logger.LogWarning(
                    $"Payment error: {lastPaymentError.Message}, " +
                    $"Code: {lastPaymentError.Code}, " +
                    $"Type: {lastPaymentError.Type}, " +
                    $"Payment Method: {lastPaymentError.PaymentMethod?.Type}"
                );
            }
            
            await _paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
            
            var order = await _unitOfWork.Repository<Order>()
                .FindAsync(o => o.PaymentIntentId == paymentIntent.Id);
                
            if (order != null) 
            {
                _logger.LogInformation($"Order {order.Id} payment failed for user {order.UserId}");
                // TODO: Send payment failure notification to customer
            }
        }
        
        private async Task HandlePaymentIntentCanceled(PaymentIntent paymentIntent)
        {
            _logger.LogWarning($"Payment canceled for intent: {paymentIntent.Id}");
            await _paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
            
            var order = await _unitOfWork.Repository<Order>()
                .FindAsync(o => o.PaymentIntentId == paymentIntent.Id);
                
            if (order != null) 
            {
                _logger.LogInformation($"Order {order.Id} payment canceled for user {order.UserId}");
                order.Status = OrderStatus.Cancelled;
                await _unitOfWork.CompleteAsync();
            }
        }
        
        private async Task HandleChargeRefunded(Charge charge)
        {
            _logger.LogInformation($"Charge refunded: {charge.Id}, PaymentIntent: {charge.PaymentIntentId}");
            
            if (!string.IsNullOrEmpty(charge.PaymentIntentId))
            {
                var order = await _unitOfWork.Repository<Order>()
                    .FindAsync(o => o.PaymentIntentId == charge.PaymentIntentId);
                    
                if (order != null)
                {
                    if (charge.Refunded)
                    {
                        order.Status = OrderStatus.Cancelled;
                        _logger.LogInformation($"Full refund processed: Order {order.Id} marked as Cancelled");
                    }
                    else if (charge.AmountRefunded > 0)
                    {
                        _logger.LogInformation($"Partial refund processed for order {order.Id}: {charge.AmountRefunded / 100m:C}");
                    }
                    
                    await _unitOfWork.CompleteAsync();
                    
                    // TODO: Send refund notification to customer
                }
            }
        }
        
        private async Task HandleChargeDisputeCreated(Dispute dispute)
        {
            _logger.LogWarning($"Dispute created for charge: {dispute.ChargeId}, Amount: {dispute.Amount / 100m:C}, Reason: {dispute.Reason}");
            
            if (!string.IsNullOrEmpty(dispute.PaymentIntentId))
            {
                var order = await _unitOfWork.Repository<Order>()
                    .FindAsync(o => o.PaymentIntentId == dispute.PaymentIntentId);
                    
                if (order != null)
                {
                    _logger.LogWarning($"Order {order.Id} has a payment dispute");
                    // TODO: Send notification to admin about the dispute
                }
            }
        }
        
        private async Task HandleChargeDisputeResolved(Dispute dispute)
        {
            _logger.LogInformation($"Dispute resolved for charge: {dispute.ChargeId}, Status: {dispute.Status}");
            
            if (!string.IsNullOrEmpty(dispute.PaymentIntentId))
            {
                var order = await _unitOfWork.Repository<Order>()
                    .FindAsync(o => o.PaymentIntentId == dispute.PaymentIntentId);
                    
                if (order != null)
                {
                    _logger.LogInformation($"Order {order.Id} dispute resolved with status: {dispute.Status}");
                    // TODO: Send notification to admin about dispute resolution
                }
            }
        }

        [HttpPut("update-status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderToReturnDto>> UpdateOrderStatus([FromBody] UpdateOrderStatusDto updateDto)
        {
            try
            {
                _logger.LogInformation($"Attempting to update order {updateDto.OrderId} status to {updateDto.Status}");
                
                // Verify the order exists
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status: {ex.Message}");
                return StatusCode(500, new ApiResponse(500, ex.Message));
            }
        }

        [HttpPost("refund")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RefundResponse>> RefundOrderAsync([FromBody] RefundOrderDto refundDto)
        {
            try
            {
                _logger.LogInformation($"Processing refund for order {refundDto.OrderId}");
                
                var order = await _orderService.GetOrderByIdAsync(refundDto.OrderId);
                if (order == null)
                {
                    return NotFound(new ApiResponse(404, "Order not found"));
                }
                
                var result = await _paymentService.CreateRefundAsync(
                    refundDto.OrderId, 
                    refundDto.Amount, 
                    refundDto.Reason
                );
                
                if (!result.Success)
                {
                    return BadRequest(new ApiResponse(400, result.Error));
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing refund for order {refundDto.OrderId}");
                return StatusCode(500, new ApiResponse(500, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
