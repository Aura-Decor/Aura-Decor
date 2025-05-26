using System.ComponentModel.DataAnnotations;

namespace AuraDecor.Core.Entities;

public class Notification : BaseEntity
{
    [Required]
    public string UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Message { get; set; }
    
    public NotificationType Type { get; set; }
    
    public bool IsRead { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ReadAt { get; set; }
    
    // Optional: Reference to related entity
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    
    // Navigation property
    public User User { get; set; }
}

public enum NotificationType
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
    OrderUpdate = 4,
    PromotionalOffer = 5,
    SystemAlert = 6,
    CartReminder = 7,
    WelcomeMessage = 8
}