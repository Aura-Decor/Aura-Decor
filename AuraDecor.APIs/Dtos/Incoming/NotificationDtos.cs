using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming;

public class CreateNotificationDto
{
    [Required]
    public string UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Message { get; set; }
    
    public int Type { get; set; } // NotificationType as int
    
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}

public class UpdateNotificationPreferencesDto
{
    public bool EmailNotifications { get; set; }
    public bool OrderUpdates { get; set; }
    public bool PromotionalOffers { get; set; }
    public bool SystemAlerts { get; set; }
    public bool CartReminders { get; set; }
}

public class BulkNotificationDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Message { get; set; }
    
    public int Type { get; set; }
    
    public List<string>? UserIds { get; set; } // Optional: if null, send to all users
}