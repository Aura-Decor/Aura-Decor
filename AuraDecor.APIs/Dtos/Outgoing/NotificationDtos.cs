namespace AuraDecor.APIs.Dtos.Outgoing;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}

public class NotificationPreferencesDto
{
    public bool EmailNotifications { get; set; }
    public bool OrderUpdates { get; set; }
    public bool PromotionalOffers { get; set; }
    public bool SystemAlerts { get; set; }
    public bool CartReminders { get; set; }
}

public class NotificationSummaryDto
{
    public int UnreadCount { get; set; }
    public List<NotificationDto> RecentNotifications { get; set; } = new();
}