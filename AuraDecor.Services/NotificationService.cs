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
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly UserManager<User> _userManager;

    public NotificationService(IUnitOfWork unitOfWork, IEmailService emailService, IEmailTemplateService emailTemplateService, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
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
            Console.WriteLine("Failed to send email notification. Please check your email settings.");
        }
    }

    private async Task SendNotificationEmailAsync(string email, string title, string message, NotificationType type)
    {
        var emailSubject = $"AuraDecor - {title}";
        var emailBody = _emailTemplateService.CreateNotificationEmailTemplate(title, message, type);

        await _emailService.SendNotificationEmailAsync(email, emailSubject, emailBody);
    }
}