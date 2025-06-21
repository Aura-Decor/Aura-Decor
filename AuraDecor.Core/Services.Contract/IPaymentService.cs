using AuraDecor.Core.Entities;
using AuraDecor.Core.Entities.Enums;
using Stripe;

namespace AuraDecor.Core.Services.Contract;

public interface IPaymentService
{
    Task<PaymentIntent> CreateOrUpdatePaymentIntentAysnc(Guid CartId);
    Task<PaymentIntentResponse> GetPaymentIntentClientSecret(Guid CartId);
    Task<bool> UpdateOrderPaymentSucceeded(string paymentIntentId);
    Task<bool> UpdateOrderPaymentFailed(string paymentIntentId);
    Task<PaymentStatus> VerifyPaymentStatus(string paymentIntentId);
    Task<RefundResponse> CreateRefundAsync(Guid orderId, decimal amount = 0, string reason = null);
}

public class PaymentIntentResponse
{
    public string ClientSecret { get; set; }
    public string PaymentIntentId { get; set; }
}

public class RefundResponse
{
    public bool Success { get; set; }
    public string RefundId { get; set; }
    public string Error { get; set; }
    public decimal Amount { get; set; }
}