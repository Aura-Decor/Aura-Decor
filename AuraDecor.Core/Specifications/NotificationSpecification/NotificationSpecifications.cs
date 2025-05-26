using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.NotificationSpecification;

public class NotificationsByUserIdSpecification : BaseSpecification<Notification>
{
    public NotificationsByUserIdSpecification(string userId, int page = 1, int pageSize = 10)
        : base(n => n.UserId == userId)
    {
        AddOrderByDesc(n => n.CreatedAt);
        ApplyPagingation((page - 1) * pageSize, pageSize);
    }
}

public class UnreadNotificationsByUserIdSpecification : BaseSpecification<Notification>
{
    public UnreadNotificationsByUserIdSpecification(string userId)
        : base(n => n.UserId == userId && !n.IsRead)
    {
        AddOrderByDesc(n => n.CreatedAt);
    }
}

public class NotificationByIdAndUserIdSpecification : BaseSpecification<Notification>
{
    public NotificationByIdAndUserIdSpecification(Guid notificationId, string userId)
        : base(n => n.Id == notificationId && n.UserId == userId)
    {
    }
}