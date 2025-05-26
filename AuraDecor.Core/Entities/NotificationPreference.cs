using System.ComponentModel.DataAnnotations;

namespace AuraDecor.Core.Entities;

public class NotificationPreference : BaseEntity
{
    [Required]
    public string UserId { get; set; }
    
    public bool EmailNotifications { get; set; } = true;
    public bool OrderUpdates { get; set; } = true;
    public bool PromotionalOffers { get; set; } = true;
    public bool SystemAlerts { get; set; } = true;
    public bool CartReminders { get; set; } = true;
    
    // Navigation property
    public User User { get; set; }
}