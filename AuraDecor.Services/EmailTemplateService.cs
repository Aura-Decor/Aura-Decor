using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;

namespace AuraDecor.Servicies;

public class EmailTemplateService : IEmailTemplateService
{
    public string CreateOtpEmailTemplate(string otp, int expiryMinutes)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    {GetBaseEmailStyles()}
                    {GetOtpSpecificStyles()}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='pattern-top'></div>
                    {GetEmailHeader()}
                    <div class='content'>
                        <p class='greeting'>Hello,</p>
                        <p class='message'>Thank you for choosing AuraDecor. We're committed to providing you with a seamless experience. To verify your account or complete your request, please use the verification code below:</p>
                        
                        <div class='feature-container'>
                            <div class='feature-label'>Verification Code</div>
                            <div class='feature-highlight'>{otp}</div>
                            <p class='feature-note'>This code will expire in {expiryMinutes} minutes</p>
                        </div>
                        
                        <p class='notice'>If you did not request this code, please disregard this email or contact our support team if you have concerns regarding your account security.</p>
                    </div>
                    {GetEmailFooter()}
                    <div class='pattern-bottom'></div>
                </div>
            </body>
            </html>";
    }

    public string CreateNotificationEmailTemplate(string title, string message, NotificationType type)
    {
        var typeColor = GetNotificationTypeColor(type);
        var typeIcon = GetNotificationTypeIcon(type);

        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    {GetBaseEmailStyles()}
                    {GetNotificationSpecificStyles(typeColor)}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='pattern-top'></div>
                    {GetEmailHeader()}
                    <div class='content'>
                        <p class='greeting'>{title}</p>
                        <p class='message'>We wanted to keep you informed about important updates regarding your AuraDecor experience.</p>
                        
                        <div class='feature-container notification-container' style='border-left-color: {typeColor};'>
                            <div class='feature-label'>Notification Details</div>
                            <div class='notification-icon-wrapper'>
                                <span class='notification-icon' style='color: {typeColor};'>{typeIcon}</span>
                            </div>
                            <div class='feature-highlight notification-message'>{message}</div>
                        </div>
                        
                        <div class='cta-section'>
                            <a href='#' class='cta-button' style='background-color: {typeColor};'>View in App</a>
                        </div>
                        
                        <p class='notice'>This notification was sent to keep you updated. If you have any questions, please contact our support team.</p>
                    </div>
                    {GetEmailFooter()}
                    <div class='pattern-bottom'></div>
                </div>
            </body>
            </html>";
    }

    public string CreateWelcomeEmailTemplate(string userName)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    {GetBaseEmailStyles()}
                    {GetWelcomeSpecificStyles()}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='pattern-top'></div>
                    {GetEmailHeader()}
                    <div class='content'>
                        <p class='greeting'>Welcome, {userName}!</p>
                        <p class='message'>We're thrilled to have you join the AuraDecor family. Get ready to transform your space with our curated collection of elegant furniture and home décor.</p>
                        
                        <div class='feature-container'>
                            <div class='feature-label'>What You'll Enjoy</div>
                            <div class='welcome-features'>
                                <div class='feature-item'>
                                    <div class='feature-icon'>🪑</div>
                                    <div class='feature-title'>Premium Furniture</div>
                                    <div class='feature-desc'>Handpicked collection of premium pieces</div>
                                </div>
                                <div class='feature-item'>
                                    <div class='feature-icon'>🚚</div>
                                    <div class='feature-title'>Free Delivery</div>
                                    <div class='feature-desc'>Free delivery on orders over $500</div>
                                </div>
                                <div class='feature-item'>
                                    <div class='feature-icon'>💎</div>
                                    <div class='feature-title'>Quality Guarantee</div>
                                    <div class='feature-desc'>100% satisfaction guarantee</div>
                                </div>
                            </div>
                        </div>
                        
                        <div class='cta-section'>
                            <a href='#' class='cta-button'>Start Shopping</a>
                        </div>
                        
                        <p class='notice'>Thank you for choosing AuraDecor. We're here to help you create the perfect ambiance for your space.</p>
                    </div>
                    {GetEmailFooter()}
                    <div class='pattern-bottom'></div>
                </div>
            </body>
            </html>";
    }

    public string CreateOrderConfirmationTemplate(string orderNumber, decimal totalAmount)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    {GetBaseEmailStyles()}
                    {GetOrderSpecificStyles()}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='pattern-top'></div>
                    {GetEmailHeader()}
                    <div class='content'>
                        <p class='greeting'>Order Confirmed!</p>
                        <p class='message'>Thank you for your order. We're preparing your items and will notify you when they're ready for delivery.</p>
                        
                        <div class='feature-container'>
                            <div class='feature-label'>Order Summary</div>
                            <div class='order-details'>
                                <div class='order-row'>
                                    <span class='order-label'>Order Number:</span>
                                    <span class='order-value'>{orderNumber}</span>
                                </div>
                                <div class='order-row'>
                                    <span class='order-label'>Total Amount:</span>
                                    <span class='order-value order-total'>${totalAmount:F2}</span>
                                </div>
                                <div class='order-row'>
                                    <span class='order-label'>Order Date:</span>
                                    <span class='order-value'>{DateTime.Now:MMM dd, yyyy}</span>
                                </div>
                            </div>
                        </div>
                        
                        <div class='cta-section'>
                            <a href='#' class='cta-button'>Track Your Order</a>
                        </div>
                        
                        <p class='notice'>You will receive tracking information once your order ships. Thank you for choosing AuraDecor!</p>
                    </div>
                    {GetEmailFooter()}
                    <div class='pattern-bottom'></div>
                </div>
            </body>
            </html>";
    }

    public string CreatePasswordResetTemplate(string resetLink)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    {GetBaseEmailStyles()}
                    {GetPasswordResetStyles()}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='pattern-top'></div>
                    {GetEmailHeader()}
                    <div class='content'>
                        <p class='greeting'>Password Reset Request</p>
                        <p class='message'>We received a request to reset your password. Click the button below to create a new password for your AuraDecor account.</p>
                        
                        <div class='feature-container reset-container'>
                            <div class='feature-label'>Security Action Required</div>
                            <div class='reset-icon-wrapper'>
                                <span class='reset-icon'>🔒</span>
                            </div>
                            <div class='reset-action'>
                                <a href='{resetLink}' class='reset-button'>Reset Password</a>
                            </div>
                            <p class='feature-note'>This link will expire in 24 hours for security reasons.</p>
                        </div>
                        
                        <p class='notice'>If you didn't request a password reset, please ignore this email or contact our support team if you have concerns about your account security.</p>
                    </div>
                    {GetEmailFooter()}
                    <div class='pattern-bottom'></div>
                </div>
            </body>
            </html>";
    }

    private string GetBaseEmailStyles()
    {
        return @"
            @import url('https://fonts.googleapis.com/css2?family=Playfair+Display:wght@400;700&family=Montserrat:wght@300;400;600&display=swap');
            
            body {
                margin: 0;
                padding: 0;
                background-color: #f5f5f5;
            }
            
            .container {
                font-family: 'Montserrat', 'Helvetica', Arial, sans-serif;
                max-width: 650px;
                margin: 20px auto;
                background-color: #ffffff;
                border: 1px solid #e0e0e0;
                box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
                position: relative;
                overflow: hidden;
            }
            
            .container:before, .container:after {
                content: '';
                position: absolute;
                width: 150px;
                height: 150px;
                background: radial-gradient(circle, #f0f0f0 20%, transparent 70%);
                z-index: 0;
            }
            
            .container:before {
                top: -50px;
                right: -50px;
            }
            
            .container:after {
                bottom: -50px;
                left: -50px;
            }
            
            .pattern-top {
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                height: 10px;
                background: repeating-linear-gradient(45deg, #000, #000 10px, transparent 10px, transparent 20px);
            }
            
            .pattern-bottom {
                position: absolute;
                bottom: 0;
                left: 0;
                right: 0;
                height: 10px;
                background: repeating-linear-gradient(-45deg, #000, #000 10px, transparent 10px, transparent 20px);
            }
            
            .header {
                background-color: #000000;
                color: white;
                padding: 40px 20px;
                text-align: center;
                position: relative;
                overflow: hidden;
            }
            
            .header-overlay {
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: linear-gradient(135deg, rgba(40, 40, 40, 0.7) 0%, rgba(0, 0, 0, 0) 100%);
                z-index: 1;
            }
            
            .header-content {
                position: relative;
                z-index: 2;
            }
            
            .logo {
                font-family: 'Playfair Display', serif;
                font-size: 36px;
                font-weight: 700;
                letter-spacing: 4px;
                margin: 0;
                text-transform: uppercase;
                border-bottom: 1px solid rgba(255, 255, 255, 0.3);
                padding-bottom: 10px;
                display: inline-block;
            }
            
            .tagline {
                font-size: 14px;
                opacity: 0.8;
                margin-top: 10px;
                font-style: italic;
            }
            
            .decorative-line {
                width: 60px;
                height: 1px;
                background-color: rgba(255, 255, 255, 0.5);
                margin: 12px auto;
            }
            
            .content {
                background-color: white;
                padding: 50px 40px;
                position: relative;
                z-index: 1;
            }
            
            .greeting {
                font-family: 'Playfair Display', serif;
                font-size: 22px;
                margin-bottom: 25px;
                color: #1a1a1a;
                border-left: 3px solid #000;
                padding-left: 15px;
            }
            
            .message {
                color: #333333;
                line-height: 1.8;
                margin-bottom: 30px;
                font-weight: 300;
                font-size: 16px;
            }
            
            .notice {
                color: #666666;
                font-size: 14px;
                margin-top: 30px;
                padding-top: 20px;
                border-top: 1px solid #eeeeee;
                font-style: italic;
                line-height: 1.7;
            }
            
            .footer {
                background-color: #f8f8f8;
                padding: 30px 20px;
                text-align: center;
                color: #666666;
                font-size: 12px;
                border-top: 1px solid #e0e0e0;
                position: relative;
                z-index: 1;
            }
            
            .social {
                margin: 20px 0;
            }
            
            .social-icon {
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
            }
            
            .social-icon:hover {
                transform: translateY(-3px);
                background-color: #333;
            }
            
            .copyright {
                margin-top: 15px;
                font-weight: 600;
                color: #333;
            }
            
            .footer-note {
                font-size: 11px;
                color: #999;
                margin-top: 10px;
            }
            
            .cta-section {
                text-align: center;
                margin: 40px 0;
            }
            
            .cta-button {
                display: inline-block;
                background-color: #000000;
                color: white;
                padding: 15px 30px;
                text-decoration: none;
                border-radius: 5px;
                font-weight: 600;
                text-transform: uppercase;
                letter-spacing: 1px;
                transition: background-color 0.3s ease;
            }
            
            .cta-button:hover {
                background-color: #333333;
            }
            
            /* Universal Feature Container - Used across all templates */
            .feature-container {
                margin: 40px 0;
                text-align: center;
                padding: 30px;
                background-color: #f8f8f8;
                border: 1px solid #e6e6e6;
                border-left: 4px solid #000;
                position: relative;
            }
            
            .feature-container:before {
                content: '';
                position: absolute;
                top: -1px;
                left: 50%;
                transform: translateX(-50%);
                width: 100px;
                height: 3px;
                background-color: #000;
            }
            
            .feature-container:after {
                content: '';
                position: absolute;
                bottom: -1px;
                left: 50%;
                transform: translateX(-50%);
                width: 100px;
                height: 3px;
                background-color: #000;
            }
            
            .feature-label {
                font-size: 12px;
                text-transform: uppercase;
                letter-spacing: 3px;
                color: #666666;
                margin-bottom: 15px;
                font-weight: 600;
            }
            
            .feature-highlight {
                font-size: 32px;
                font-weight: bold;
                color: #000000;
                margin: 15px 0;
                padding: 10px 20px;
                display: inline-block;
                position: relative;
            }
            
            .feature-note {
                color: #666666;
                text-align: center;
                font-size: 14px;
                margin-top: 15px;
                font-style: italic;
            }";
    }

    private string GetOtpSpecificStyles()
    {
        return @"
            .feature-highlight {
                font-size: 40px;
                letter-spacing: 12px;
                font-family: 'Courier New', monospace;
            }
            
            .feature-highlight:before, .feature-highlight:after {
                content: '';
                position: absolute;
                width: 15px;
                height: 15px;
                border: 2px solid #000;
            }
            
            .feature-highlight:before {
                top: 0;
                left: 0;
                border-right: none;
                border-bottom: none;
            }
            
            .feature-highlight:after {
                bottom: 0;
                right: 0;
                border-left: none;
                border-top: none;
            }";
    }

    private string GetNotificationSpecificStyles(string typeColor)
    {
        return $@"
            .notification-container {{
                border-left-color: {typeColor};
            }}
            
            .notification-icon-wrapper {{
                margin: 15px 0;
            }}
            
            .notification-icon {{
                font-size: 48px;
                display: inline-block;
            }}
            
            .notification-message {{
                font-size: 18px;
                line-height: 1.6;
                color: #333;
                margin: 15px 0;
            }}";
    }

    private string GetWelcomeSpecificStyles()
    {
        return @"
            .welcome-features {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
                gap: 20px;
                margin-top: 20px;
            }
            
            .feature-item {
                text-align: center;
                padding: 15px;
            }
            
            .feature-icon {
                font-size: 24px;
                margin-bottom: 10px;
                display: block;
            }
            
            .feature-title {
                font-size: 14px;
                font-weight: 600;
                margin-bottom: 8px;
                color: #333;
            }
            
            .feature-desc {
                font-size: 12px;
                color: #666;
                line-height: 1.4;
            }";
    }

    private string GetOrderSpecificStyles()
    {
        return @"
            .order-details {
                margin-top: 20px;
                text-align: left;
            }
            
            .order-row {
                display: flex;
                justify-content: space-between;
                padding: 12px 0;
                border-bottom: 1px solid #eee;
            }
            
            .order-row:last-child {
                border-bottom: none;
                font-weight: 600;
            }
            
            .order-label {
                color: #666;
                font-size: 14px;
            }
            
            .order-value {
                color: #333;
                font-weight: 500;
                font-size: 14px;
            }
            
            .order-total {
                font-size: 16px;
                font-weight: 700;
                color: #000;
            }";
    }

    private string GetPasswordResetStyles()
    {
        return @"
            .reset-container {
                border-left-color: #dc3545;
            }
            
            .reset-icon-wrapper {
                margin: 15px 0;
            }
            
            .reset-icon {
                font-size: 32px;
                color: #dc3545;
                display: inline-block;
            }
            
            .reset-action {
                margin: 20px 0;
            }
            
            .reset-button {
                display: inline-block;
                background-color: #dc3545;
                color: white;
                padding: 12px 25px;
                text-decoration: none;
                border-radius: 5px;
                font-weight: 600;
                text-transform: uppercase;
                letter-spacing: 1px;
                font-size: 14px;
            }
            
            .reset-button:hover {
                background-color: #c82333;
            }";
    }

    private string GetEmailHeader()
    {
        return @"
            <div class='header'>
                <div class='header-overlay'></div>
                <div class='header-content'>
                    <h1 class='logo'>AURADECOR</h1>
                    <div class='decorative-line'></div>
                    <div class='tagline'>Elegance in Every Detail</div>
                </div>
            </div>";
    }

    private string GetEmailFooter()
    {
        return $@"
            <div class='footer'>
                <div class='social'>
                    <a href='#' class='social-icon'>f</a>
                    <a href='#' class='social-icon'>in</a>
                    <a href='#' class='social-icon'>ig</a>
                </div>
                <p class='copyright'>© {DateTime.Now.Year} AuraDecor. All Rights Reserved.</p>
                <p class='footer-note'>This is an automated message, please do not reply.</p>
            </div>";
    }

    private string GetNotificationTypeColor(NotificationType type)
    {
        return type switch
        {
            NotificationType.Success => "#28a745",
            NotificationType.Error => "#dc3545",
            NotificationType.Warning => "#ffc107",
            NotificationType.OrderUpdate => "#007bff",
            NotificationType.PromotionalOffer => "#17a2b8",
            NotificationType.SystemAlert => "#6c757d",
            NotificationType.CartReminder => "#fd7e14",
            NotificationType.WelcomeMessage => "#28a745",
            _ => "#6c757d"
        };
    }

    private string GetNotificationTypeIcon(NotificationType type)
    {
        return type switch
        {
            NotificationType.Success => "✅",
            NotificationType.Error => "❌",
            NotificationType.Warning => "⚠️",
            NotificationType.OrderUpdate => "📦",
            NotificationType.PromotionalOffer => "🎁",
            NotificationType.SystemAlert => "🔔",
            NotificationType.CartReminder => "🛒",
            NotificationType.WelcomeMessage => "👋",
            _ => "ℹ️"
        };
    }
}