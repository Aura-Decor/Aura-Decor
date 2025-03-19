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
            Subject = "Your AuraDecor Verification Code",
            Body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        @import url('https://fonts.googleapis.com/css2?family=Playfair+Display:wght@400;700&family=Montserrat:wght@300;400;600&display=swap');
                        
                        body {{
                            margin: 0;
                            padding: 0;
                            background-color: #f5f5f5;
                        }}
                        
                        .container {{
                            font-family: 'Montserrat', 'Helvetica', Arial, sans-serif;
                            max-width: 650px;
                            margin: 20px auto;
                            background-color: #ffffff;
                            border: 1px solid #e0e0e0;
                            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
                            position: relative;
                            overflow: hidden;
                        }}
                        
                        .container:before, .container:after {{
                            content: '';
                            position: absolute;
                            width: 150px;
                            height: 150px;
                            background: radial-gradient(circle, #f0f0f0 20%, transparent 70%);
                            z-index: 0;
                        }}
                        
                        .container:before {{
                            top: -50px;
                            right: -50px;
                        }}
                        
                        .container:after {{
                            bottom: -50px;
                            left: -50px;
                        }}
                        
                        .pattern-top {{
                            position: absolute;
                            top: 0;
                            left: 0;
                            right: 0;
                            height: 10px;
                            background: repeating-linear-gradient(45deg, #000, #000 10px, transparent 10px, transparent 20px);
                        }}
                        
                        .pattern-bottom {{
                            position: absolute;
                            bottom: 0;
                            left: 0;
                            right: 0;
                            height: 10px;
                            background: repeating-linear-gradient(-45deg, #000, #000 10px, transparent 10px, transparent 20px);
                        }}
                        
                        .header {{
                            background-color: #000000;
                            color: white;
                            padding: 40px 20px;
                            text-align: center;
                            position: relative;
                            overflow: hidden;
                        }}
                        
                        .header-overlay {{
                            position: absolute;
                            top: 0;
                            left: 0;
                            right: 0;
                            bottom: 0;
                            background: linear-gradient(135deg, rgba(40, 40, 40, 0.7) 0%, rgba(0, 0, 0, 0) 100%);
                            z-index: 1;
                        }}
                        
                        .header-content {{
                            position: relative;
                            z-index: 2;
                        }}
                        
                        .logo {{
                            font-family: 'Playfair Display', serif;
                            font-size: 36px;
                            font-weight: 700;
                            letter-spacing: 4px;
                            margin: 0;
                            text-transform: uppercase;
                            border-bottom: 1px solid rgba(255, 255, 255, 0.3);
                            padding-bottom: 10px;
                            display: inline-block;
                        }}
                        
                        .tagline {{
                            font-size: 14px;
                            font-weight: 300;
                            letter-spacing: 2px;
                            margin-top: 10px;
                            color: #cccccc;
                            text-transform: uppercase;
                        }}
                        
                        .decorative-line {{
                            width: 60px;
                            height: 1px;
                            background-color: rgba(255, 255, 255, 0.5);
                            margin: 12px auto;
                        }}
                        
                        .content {{
                            background-color: white;
                            padding: 50px 40px;
                            position: relative;
                            z-index: 1;
                        }}
                        
                        .greeting {{
                            font-family: 'Playfair Display', serif;
                            font-size: 22px;
                            margin-bottom: 25px;
                            color: #1a1a1a;
                            border-left: 3px solid #000;
                            padding-left: 15px;
                        }}
                        
                        .message {{
                            color: #333333;
                            line-height: 1.8;
                            margin-bottom: 30px;
                            font-weight: 300;
                            font-size: 16px;
                        }}
                        
                        .otp-container {{
                            margin: 40px 0;
                            text-align: center;
                            padding: 30px;
                            background-color: #f8f8f8;
                            border: 1px solid #e6e6e6;
                            position: relative;
                        }}
                        
                        .otp-container:before {{
                            content: '';
                            position: absolute;
                            top: -1px;
                            left: 50%;
                            transform: translateX(-50%);
                            width: 100px;
                            height: 3px;
                            background-color: #000;
                        }}
                        
                        .otp-container:after {{
                            content: '';
                            position: absolute;
                            bottom: -1px;
                            left: 50%;
                            transform: translateX(-50%);
                            width: 100px;
                            height: 3px;
                            background-color: #000;
                        }}
                        
                        .otp-label {{
                            font-size: 12px;
                            text-transform: uppercase;
                            letter-spacing: 3px;
                            color: #666666;
                            margin-bottom: 15px;
                            font-weight: 600;
                        }}
                        
                        .otp-code {{
                            font-size: 40px;
                            font-weight: bold;
                            color: #000000;
                            letter-spacing: 12px;
                            margin: 15px 0;
                            font-family: 'Courier New', monospace;
                            padding: 10px 20px;
                            display: inline-block;
                            position: relative;
                        }}
                        
                        .otp-code:before, .otp-code:after {{
                            content: '';
                            position: absolute;
                            width: 15px;
                            height: 15px;
                            border: 2px solid #000;
                        }}
                        
                        .otp-code:before {{
                            top: 0;
                            left: 0;
                            border-right: none;
                            border-bottom: none;
                        }}
                        
                        .otp-code:after {{
                            bottom: 0;
                            right: 0;
                            border-left: none;
                            border-top: none;
                        }}
                        
                        .expiry {{
                            color: #666666;
                            text-align: center;
                            font-size: 14px;
                            margin-top: 15px;
                            font-style: italic;
                        }}
                        
                        .notice {{
                            color: #666666;
                            font-size: 14px;
                            margin-top: 30px;
                            padding-top: 20px;
                            border-top: 1px solid #eeeeee;
                            font-style: italic;
                            line-height: 1.7;
                        }}
                        
                        .footer {{
                            background-color: #f8f8f8;
                            padding: 30px 20px;
                            text-align: center;
                            color: #666666;
                            font-size: 12px;
                            border-top: 1px solid #e0e0e0;
                            position: relative;
                            z-index: 1;
                        }}
                        
                        .social {{
                            margin: 20px 0;
                        }}
                        
                        .social-icon {{
                            display: inline-block;
                            width: 38px;
                            height: 38px;
                            background-color: #000000;
                            border-radius: 50%;
                            margin: 0 6px;
                            text-align: center;
                            line-height: 38px;
                            color: white;
                            text-decoration: none;
                            font-weight: bold;
                            box-shadow: 0 3px 5px rgba(0,0,0,0.1);
                            transition: transform 0.3s ease, background-color 0.3s ease;
                        }}
                        
                        .social-icon:hover {{
                            transform: translateY(-3px);
                            background-color: #333;
                        }}
                        
                        .copyright {{
                            margin-top: 15px;
                            font-weight: 600;
                            color: #333;
                        }}
                        
                        .footer-note {{
                            font-size: 11px;
                            color: #999;
                            margin-top: 10px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='pattern-top'></div>
                        <div class='header'>
                            <div class='header-overlay'></div>
                            <div class='header-content'>
                                <h1 class='logo'>AURADECOR</h1>
                                <div class='decorative-line'></div>
                                <div class='tagline'>Elegance in Every Detail</div>
                            </div>
                        </div>
                        <div class='content'>
                            <p class='greeting'>Hello,</p>
                            <p class='message'>Thank you for choosing AuraDecor. We're committed to providing you with a seamless experience. To verify your account or complete your request, please use the verification code below:</p>
                            
                            <div class='otp-container'>
                                <div class='otp-label'>Verification Code</div>
                                <div class='otp-code'>{otp}</div>
                                <p class='expiry'>This code will expire in {OTP_EXPIRY} minutes</p>
                            </div>
                            
                            <p class='notice'>If you did not request this code, please disregard this email or contact our support team if you have concerns regarding your account security.</p>
                        </div>
                        <div class='footer'>
                            <div class='social'>
                                <a href='#' class='social-icon'>f</a>
                                <a href='#' class='social-icon'>in</a>
                                <a href='#' class='social-icon'>ig</a>
                            </div>
                            <p class='copyright'>© {DateTime.Now.Year} AuraDecor. All Rights Reserved.</p>
                            <p class='footer-note'>This is an automated message, please do not reply.</p>
                        </div>
                        <div class='pattern-bottom'></div>
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