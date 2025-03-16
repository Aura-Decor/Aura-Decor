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
    private const int OTP_EXPIRY = 5;

    public EmailService(IOptions<EmailSettings> emailSettings, IConnectionMultiplexer redis)
    {
        _emailSettings = emailSettings.Value;
        _redisDb = redis.GetDatabase();
    }
public async Task SendOtpEmailAsync(string email, string otp)
{
    var message = new MailMessage
    {
        From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
        Subject = "Your OTP Code",
        Body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    .container {{
                        font-family: 'Arial', sans-serif;
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        background-color: #f8f9fa;
                        border-radius: 10px;
                    }}
                    .header {{
                        background-color: #4a90e2;
                        color: white;
                        padding: 20px;
                        text-align: center;
                        border-radius: 5px 5px 0 0;
                    }}
                    .content {{
                        background-color: white;
                        padding: 30px;
                        border-radius: 0 0 5px 5px;
                    }}
                    .otp-code {{
                        font-size: 32px;
                        font-weight: bold;
                        color: #4a90e2;
                        text-align: center;
                        letter-spacing: 5px;
                        margin: 20px 0;
                        padding: 10px;
                        background-color: #f8f9fa;
                        border-radius: 5px;
                    }}
                    .expiry {{
                        color: #666;
                        text-align: center;
                        font-size: 14px;
                    }}
                    .footer {{
                        margin-top: 20px;
                        text-align: center;
                        color: #666;
                        font-size: 12px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>One-Time Password</h1>
                    </div>
                    <div class='content'>
                        <p>Hello,</p>
                        <p>Your verification code is:</p>
                        <div class='otp-code'>{otp}</div>
                        <p class='expiry'>This code will expire in {OTP_EXPIRY} minutes</p>
                        <p>If you didn't request this code, please ignore this email.</p>
                        <div class='footer'>
                            <p>This is an automated message, please do not reply.</p>
                            <p>&copy; {DateTime.Now.Year} AuraDecor. All rights reserved.</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>",
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
}