using Microsoft.AspNetCore.Identity;

namespace AuraDecor.Core.Entities;

public class User : IdentityUser
{
    public string DisplayName { get; set; }
    public Address Address { get; set; }
    
    // Navigation properties for notifications
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public NotificationPreference NotificationPreference { get; set; }
}