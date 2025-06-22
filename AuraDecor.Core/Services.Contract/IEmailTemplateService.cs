using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Services.Contract;

public interface IEmailTemplateService
{
    string CreateOtpEmailTemplate(string otp, int expiryMinutes);
    string CreateNotificationEmailTemplate(string title, string message, NotificationType type);
    string CreateWelcomeEmailTemplate(string userName);
    string CreateOrderConfirmationTemplate(string orderNumber, decimal totalAmount);
    string CreatePasswordResetTemplate(string resetLink);
    string CreatePaymentFailureTemplate(string orderNumber, string failureReason);
    string CreateRefundNotificationTemplate(string orderNumber, decimal refundAmount, string refundReason);
    string CreateAdminDisputeNotificationTemplate(string orderNumber, string disputeReason, decimal disputeAmount);
}