namespace AuraDecor.Core.Messages;

public class OtpEmailMessage : EmailMessage
{
    public string Otp { get; set; }
    public int ExpiryMinutes { get; set; }
}