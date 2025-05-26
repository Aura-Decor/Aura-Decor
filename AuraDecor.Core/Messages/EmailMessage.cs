namespace AuraDecor.Core.Messages;

public class EmailMessage
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string HtmlBody { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}