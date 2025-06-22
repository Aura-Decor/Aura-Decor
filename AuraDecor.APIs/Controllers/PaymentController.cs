using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.APIs.Helpers;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications.OrderSpecification;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using AuraDecor.Core.Entities.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuraDecor.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IResponseCacheService _cacheService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly UserManager<User> _userManager;

        public PaymentController(
            IPaymentService paymentService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PaymentController> logger,
            IConfiguration configuration,
            IResponseCacheService cacheService,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            UserManager<User> userManager)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _cacheService = cacheService;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _userManager = userManager;
        }

        [HttpGet("verify/{paymentIntentId}")]
        public async Task<ActionResult<string>> VerifyPaymentStatus(string paymentIntentId)
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
                
                await CacheInvalidationHelper.InvalidateOrderCacheAsync(_cacheService);
                await CacheInvalidationHelper.InvalidatePaymentCacheAsync(_cacheService);
                await CacheInvalidationHelper.InvalidateUserSpecificCacheAsync(_cacheService, userId);
                await CacheInvalidationHelper.InvalidateCartCacheAsync(_cacheService);
            }
            else if (paymentStatus == PaymentStatus.Failed)
            {
                await _paymentService.UpdateOrderPaymentFailed(paymentIntentId);

                await CacheInvalidationHelper.InvalidateOrderCacheAsync(_cacheService);
                await CacheInvalidationHelper.InvalidatePaymentCacheAsync(_cacheService);
            }
            
            return Ok(new { status = paymentStatus.ToString() });
        }
        
        [HttpGet("order-details/{paymentIntentId}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByPaymentIntent(string paymentIntentId)
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
                throw; 
            }
            
            return Ok();
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

                    await CacheInvalidationHelper.InvalidateOrderCacheAsync(_cacheService);
                    await CacheInvalidationHelper.InvalidatePaymentCacheAsync(_cacheService);
                    await CacheInvalidationHelper.InvalidateUserSpecificCacheAsync(_cacheService, order.UserId);
                    await CacheInvalidationHelper.InvalidateCartCacheAsync(_cacheService);
                    await CacheInvalidationHelper.InvalidateInventoryRelatedCacheAsync(_cacheService);
                    
                    // Send order confirmation email to customer
                    try
                    {
                        var user = await _userManager.FindByIdAsync(order.UserId);
                        if (user?.Email != null)
                        {
                            var emailSubject = "AuraDecor - Order Confirmation";
                            var emailBody = _emailTemplateService.CreateOrderConfirmationTemplate(
                                order.Id.ToString(), 
                                order.OrderAmount
                            );
                            
                            await _emailService.SendNotificationEmailAsync(user.Email, emailSubject, emailBody);
                            _logger.LogInformation($"Order confirmation email sent to {user.Email} for order {order.Id}");
                        }
                        else
                        {
                            _logger.LogWarning($"Could not send order confirmation email - user or email not found for order {order.Id}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send order confirmation email for order {order.Id}");
                    }
                    
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

                // Invalidate cache after failed payment
                await CacheInvalidationHelper.InvalidateOrderCacheAsync(_cacheService);
                await CacheInvalidationHelper.InvalidatePaymentCacheAsync(_cacheService);
                await CacheInvalidationHelper.InvalidateUserSpecificCacheAsync(_cacheService, order.UserId);
                
                // Send payment failure notification to customer
                try
                {
                    var user = await _userManager.FindByIdAsync(order.UserId);
                    if (user?.Email != null)
                    {
                        var failureReason = lastPaymentError?.Message ?? "Payment could not be processed";
                        var emailSubject = "AuraDecor - Payment Failed";
                        var emailBody = _emailTemplateService.CreatePaymentFailureTemplate(
                            order.Id.ToString(),
                            failureReason
                        );
                        
                        await _emailService.SendNotificationEmailAsync(user.Email, emailSubject, emailBody);
                        _logger.LogInformation($"Payment failure email sent to {user.Email} for order {order.Id}");
                    }
                    else
                    {
                        _logger.LogWarning($"Could not send payment failure email - user or email not found for order {order.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send payment failure email for order {order.Id}");
=                }
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

                // Invalidate cache after payment cancellation
                await CacheInvalidationHelper.InvalidateOrderCacheAsync(_cacheService);
                await CacheInvalidationHelper.InvalidatePaymentCacheAsync(_cacheService);
                await CacheInvalidationHelper.InvalidateUserSpecificCacheAsync(_cacheService, order.UserId);
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

                    // Invalidate cache after refund
                    await CacheInvalidationHelper.InvalidateOrderCacheAsync(_cacheService);
                    await CacheInvalidationHelper.InvalidatePaymentCacheAsync(_cacheService);
                    await CacheInvalidationHelper.InvalidateUserSpecificCacheAsync(_cacheService, order.UserId);
                    await CacheInvalidationHelper.InvalidateInventoryRelatedCacheAsync(_cacheService);
                    
                    // Send refund notification to customer
                    try
                    {
                        var user = await _userManager.FindByIdAsync(order.UserId);
                        if (user?.Email != null)
                        {
                            var refundAmount = charge.AmountRefunded > 0 ? charge.AmountRefunded / 100m : charge.Amount / 100m;
                            var refundReason = charge.Refunded ? "Full refund processed" : "Partial refund processed";
                            var emailSubject = "AuraDecor - Refund Processed";
                            var emailBody = _emailTemplateService.CreateRefundNotificationTemplate(
                                order.Id.ToString(),
                                refundAmount,
                                refundReason
                            );
                            
                            await _emailService.SendNotificationEmailAsync(user.Email, emailSubject, emailBody);
                            _logger.LogInformation($"Refund notification email sent to {user.Email} for order {order.Id}");
                        }
                        else
                        {
                            _logger.LogWarning($"Could not send refund notification email - user or email not found for order {order.Id}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send refund notification email for order {order.Id}");
=                    }
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
                    
                    // Send notification to admin about the dispute
                    try
                    {
                        // Get admin users
                        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                        
                        foreach (var admin in adminUsers)
                        {
                            if (!string.IsNullOrEmpty(admin.Email))
                            {
                                var emailSubject = "AuraDecor Admin - Payment Dispute Alert";
                                var emailBody = _emailTemplateService.CreateAdminDisputeNotificationTemplate(
                                    order.Id.ToString(),
                                    dispute.Reason,
                                    dispute.Amount / 100m
                                );
                                
                                await _emailService.SendNotificationEmailAsync(admin.Email, emailSubject, emailBody);
                                _logger.LogInformation($"Dispute notification email sent to admin {admin.Email} for order {order.Id}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send dispute notification email for order {order.Id}");
                    }
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
                    
                    // Send notification to admin about dispute resolution
                    try
                    {
                        // Get admin users
                        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                        
                        foreach (var admin in adminUsers)
                        {
                            if (!string.IsNullOrEmpty(admin.Email))
                            {
                                var emailSubject = "AuraDecor Admin - Dispute Resolved";
                                var emailBody = _emailTemplateService.CreateNotificationEmailTemplate(
                                    "Payment Dispute Resolved",
                                    $"The payment dispute for order #{order.Id} has been resolved with status: {dispute.Status}. Please review the resolution details in the admin panel.",
                                    NotificationType.SystemAlert
                                );
                                
                                await _emailService.SendNotificationEmailAsync(admin.Email, emailSubject, emailBody);
                                _logger.LogInformation($"Dispute resolution notification email sent to admin {admin.Email} for order {order.Id}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send dispute resolution notification email for order {order.Id}");
                    }
                }
            }
        }

        [HttpPost("refund")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RefundResponse>> RefundOrderAsync([FromBody] RefundOrderDto refundDto)
        {
            _logger.LogInformation($"Processing refund for order {refundDto.OrderId}");
            
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(refundDto.OrderId);
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

            // Invalidate cache after admin refund
            await CacheInvalidationHelper.InvalidateOrderCacheAsync(_cacheService);
            await CacheInvalidationHelper.InvalidatePaymentCacheAsync(_cacheService);
            await CacheInvalidationHelper.InvalidateAdminCacheAsync(_cacheService);
            await CacheInvalidationHelper.InvalidateUserSpecificCacheAsync(_cacheService, order.UserId);
            await CacheInvalidationHelper.InvalidateInventoryRelatedCacheAsync(_cacheService);
            
            return Ok(result);
        }
    }
}
