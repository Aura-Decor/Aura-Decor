using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Services.Contract;

public interface INotificationService
{
    // Notification CRUD operations
    Task<Notification> CreateNotificationAsync(string userId, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null);
    Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 10);
    Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(string userId);
    Task<int> GetUnreadNotificationCountAsync(string userId);
    Task<bool> MarkAsReadAsync(Guid notificationId, string userId);
    Task<bool> MarkAllAsReadAsync(string userId);
    Task<bool> DeleteNotificationAsync(Guid notificationId, string userId);
    Task<bool> DeleteAllNotificationsAsync(string userId);
    
    // Notification preferences
    Task<NotificationPreference> GetUserNotificationPreferencesAsync(string userId);
    Task<bool> UpdateNotificationPreferencesAsync(string userId, NotificationPreference preferences);
    
    // Bulk notification operations
    Task SendNotificationToAllUsersAsync(string title, string message, NotificationType type);
    Task SendNotificationToUsersAsync(IEnumerable<string> userIds, string title, string message, NotificationType type);
    
    // Specific notification types
    Task SendWelcomeNotificationAsync(string userId);
    Task SendOrderUpdateNotificationAsync(string userId, Guid orderId, string orderStatus);
    Task SendPromotionalOfferNotificationAsync(string userId, string offerDetails);
    Task SendCartReminderNotificationAsync(string userId);
    Task SendSystemAlertAsync(string userId, string alertMessage);
}