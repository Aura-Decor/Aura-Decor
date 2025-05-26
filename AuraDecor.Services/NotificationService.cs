using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications.NotificationSpecification;
using Microsoft.AspNetCore.Identity;

namespace AuraDecor.Servicies;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;

    public NotificationService(IUnitOfWork unitOfWork, IEmailService emailService, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _userManager = userManager;
    }

    public async Task<Notification> CreateNotificationAsync(string userId, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
            CreatedAt = DateTime.UtcNow
        };

        _unitOfWork.Repository<Notification>().Add(notification);
        await _unitOfWork.CompleteAsync();

        // Send email notification if user preferences allow
        await SendEmailNotificationIfEnabled(userId, title, message, type);

        return notification;
    }

    public async Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 10)
    {
        var spec = new NotificationsByUserIdSpecification(userId, page, pageSize);
        return await _unitOfWork.Repository<Notification>().GetAllWithSpecAsync(spec);
    }

    public async Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(string userId)
    {
        var spec = new UnreadNotificationsByUserIdSpecification(userId);
        return await _unitOfWork.Repository<Notification>().GetAllWithSpecAsync(spec);
    }

    public async Task<int> GetUnreadNotificationCountAsync(string userId)
    {
        var spec = new UnreadNotificationsByUserIdSpecification(userId);
        var notifications = await _unitOfWork.Repository<Notification>().GetAllWithSpecAsync(spec);
        return notifications.Count;
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId, string userId)
    {
        var spec = new NotificationByIdAndUserIdSpecification(notificationId, userId);
        var notification = await _unitOfWork.Repository<Notification>().GetWithSpecAsync(spec);

        if (notification == null || notification.IsRead)
            return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Notification>().UpdateAsync(notification);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(string userId)
    {
        var spec = new UnreadNotificationsByUserIdSpecification(userId);
        var unreadNotifications = await _unitOfWork.Repository<Notification>().GetAllWithSpecAsync(spec);

        if (!unreadNotifications.Any())
            return false;

        var readTime = DateTime.UtcNow;
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = readTime;
            await _unitOfWork.Repository<Notification>().UpdateAsync(notification);
        }

        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> DeleteNotificationAsync(Guid notificationId, string userId)
    {
        var spec = new NotificationByIdAndUserIdSpecification(notificationId, userId);
        var notification = await _unitOfWork.Repository<Notification>().GetWithSpecAsync(spec);

        if (notification == null)
            return false;

        _unitOfWork.Repository<Notification>().DeleteAsync(notification);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> DeleteAllNotificationsAsync(string userId)
    {
        var spec = new NotificationsByUserIdSpecification(userId, 1, int.MaxValue);
        var notifications = await _unitOfWork.Repository<Notification>().GetAllWithSpecAsync(spec);

        if (!notifications.Any())
            return false;

        foreach (var notification in notifications)
        {
            _unitOfWork.Repository<Notification>().DeleteAsync(notification);
        }

        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<NotificationPreference> GetUserNotificationPreferencesAsync(string userId)
    {
        var preferences = await _unitOfWork.Repository<NotificationPreference>()
            .GetAllAsync();
        
        var userPreference = preferences.FirstOrDefault(p => p.UserId == userId);
        
        if (userPreference == null)
        {
            // Create default preferences for new user
            userPreference = new NotificationPreference
            {
                UserId = userId,
                EmailNotifications = true,
                OrderUpdates = true,
                PromotionalOffers = true,
                SystemAlerts = true,
                CartReminders = true
            };

            _unitOfWork.Repository<NotificationPreference>().Add(userPreference);
            await _unitOfWork.CompleteAsync();
        }

        return userPreference;
    }

    public async Task<bool> UpdateNotificationPreferencesAsync(string userId, NotificationPreference preferences)
    {
        var existingPreferences = await GetUserNotificationPreferencesAsync(userId);
        
        existingPreferences.EmailNotifications = preferences.EmailNotifications;
        existingPreferences.OrderUpdates = preferences.OrderUpdates;
        existingPreferences.PromotionalOffers = preferences.PromotionalOffers;
        existingPreferences.SystemAlerts = preferences.SystemAlerts;
        existingPreferences.CartReminders = preferences.CartReminders;

        await _unitOfWork.Repository<NotificationPreference>().UpdateAsync(existingPreferences);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task SendNotificationToAllUsersAsync(string title, string message, NotificationType type)
    {
        var users = _userManager.Users.ToList();
        
        foreach (var user in users)
        {
            await CreateNotificationAsync(user.Id, title, message, type);
        }
    }

    public async Task SendNotificationToUsersAsync(IEnumerable<string> userIds, string title, string message, NotificationType type)
    {
        foreach (var userId in userIds)
        {
            await CreateNotificationAsync(userId, title, message, type);
        }
    }

    public async Task SendWelcomeNotificationAsync(string userId)
    {
        await CreateNotificationAsync(
            userId,
            "Welcome to AuraDecor!",
            "Thank you for joining AuraDecor. Discover our elegant furniture collection and transform your space with style.",
            NotificationType.WelcomeMessage
        );
    }

    public async Task SendOrderUpdateNotificationAsync(string userId, Guid orderId, string orderStatus)
    {
        await CreateNotificationAsync(
            userId,
            "Order Status Update",
            $"Your order has been updated to: {orderStatus}",
            NotificationType.OrderUpdate,
            orderId.ToString(),
            "Order"
        );
    }

    public async Task SendPromotionalOfferNotificationAsync(string userId, string offerDetails)
    {
        await CreateNotificationAsync(
            userId,
            "Special Offer Available!",
            offerDetails,
            NotificationType.PromotionalOffer
        );
    }

    public async Task SendCartReminderNotificationAsync(string userId)
    {
        await CreateNotificationAsync(
            userId,
            "Don't forget your cart!",
            "You have items waiting in your cart. Complete your purchase before they're gone!",
            NotificationType.CartReminder
        );
    }

    public async Task SendSystemAlertAsync(string userId, string alertMessage)
    {
        await CreateNotificationAsync(
            userId,
            "System Alert",
            alertMessage,
            NotificationType.SystemAlert
        );
    }

    private async Task SendEmailNotificationIfEnabled(string userId, string title, string message, NotificationType type)
    {
        try
        {
            var preferences = await GetUserNotificationPreferencesAsync(userId);
            
            if (!preferences.EmailNotifications)
                return;

            // Check specific notification type preferences
            bool shouldSendEmail = type switch
            {
                NotificationType.OrderUpdate => preferences.OrderUpdates,
                NotificationType.PromotionalOffer => preferences.PromotionalOffers,
                NotificationType.SystemAlert => preferences.SystemAlerts,
                NotificationType.CartReminder => preferences.CartReminders,
                _ => true
            };

            if (!shouldSendEmail)
                return;

            var user = await _userManager.FindByIdAsync(userId);
            if (user?.Email != null)
            {
                await SendNotificationEmailAsync(user.Email, title, message, type);
            }
        }
        catch (Exception)
        {
            // Log error but don't fail notification creation
            // Could implement proper logging here
        }
    }

    private async Task SendNotificationEmailAsync(string email, string title, string message, NotificationType type)
    {
        // Create a styled email for notifications
        var emailSubject = $"AuraDecor - {title}";
        var emailBody = CreateNotificationEmailTemplate(title, message, type);

        // Note: You might want to extend IEmailService to support custom email templates
        // For now, we'll use a simple approach
        await Task.Run(() => {
            // This is a placeholder - you would need to extend EmailService
            // to support notification emails with custom templates
        });
    }

    private string CreateNotificationEmailTemplate(string title, string message, NotificationType type)
    {
        var typeColor = type switch
        {
            NotificationType.Success => "#28a745",
            NotificationType.Error => "#dc3545",
            NotificationType.Warning => "#ffc107",
            NotificationType.OrderUpdate => "#007bff",
            NotificationType.PromotionalOffer => "#17a2b8",
            _ => "#6c757d"
        };

        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                    .container {{ max-width: 600px; margin: 0 auto; background-color: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                    .header {{ background-color: {typeColor}; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 30px; }}
                    .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; color: #6c757d; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>AuraDecor</h1>
                        <h2>{title}</h2>
                    </div>
                    <div class='content'>
                        <p>{message}</p>
                    </div>
                    <div class='footer'>
                        <p>© 2025 AuraDecor. All rights reserved.</p>
                        <p>This is an automated notification from AuraDecor.</p>
                    </div>
                </div>
            </body>
            </html>";
    }
}