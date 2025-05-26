using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Messages;

public class NotificationEmailMessage : EmailMessage
{
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType NotificationType { get; set; }
}