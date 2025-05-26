using System.Net;
using System.Net.Mail;
using AuraDecor.Core.Configuration;
using AuraDecor.Core.Services.Contract;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AuraDecor.Servicies;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly IDatabase _redisDb;
    private readonly IEmailTemplateService _emailTemplateService;
    private const int OTP_EXPIRY = 5;

    public EmailService(IOptions<EmailSettings> emailSettings, IConnectionMultiplexer redis, IEmailTemplateService emailTemplateService)
    {
        _emailSettings = emailSettings.Value;
        _redisDb = redis.GetDatabase();
        _emailTemplateService = emailTemplateService;
    }

    public async Task SendOtpEmailAsync(string email, string otp)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
            Subject = "Your AuraDecor Verification Code",
            Body = _emailTemplateService.CreateOtpEmailTemplate(otp, OTP_EXPIRY),
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(email));

        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
        };

        await client.SendMailAsync(message);

        await _redisDb.StringSetAsync(
            $"otp:{email}",
            otp,
            TimeSpan.FromMinutes(OTP_EXPIRY)
        );
    }

    public async Task<bool> ValidateOtpAsync(string email, string otp)
    {
        var storedotp = await _redisDb.StringGetAsync($"otp:{email}");
        return !storedotp.IsNull && storedotp.ToString() == otp;
    }

    public async Task SendNotificationEmailAsync(string email, string subject, string htmlBody)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(email));

        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
        };

        await client.SendMailAsync(message);
    }
}