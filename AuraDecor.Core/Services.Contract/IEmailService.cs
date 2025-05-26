namespace AuraDecor.Core.Services.Contract;

public interface IEmailService
{
    Task<bool> ValidateOtpAsync(string email, string otp);
    Task SendOtpEmailAsync(string email, string otp);
    Task SendNotificationEmailAsync(string email, string subject, string htmlBody);
}