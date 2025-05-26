using AuraDecor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuraDecor.Repository.Data.Config;

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.UserId)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.IsRead)
            .HasDefaultValue(false);

        // Configure relationship with User
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Add indexes for performance
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);
        builder.HasIndex(n => n.CreatedAt);
        builder.HasIndex(n => new { n.UserId, n.IsRead });
    }
}

public class NotificationPreferenceConfig : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.Property(np => np.UserId)
            .IsRequired();

        builder.Property(np => np.EmailNotifications)
            .HasDefaultValue(true);

        builder.Property(np => np.OrderUpdates)
            .HasDefaultValue(true);

        builder.Property(np => np.PromotionalOffers)
            .HasDefaultValue(true);

        builder.Property(np => np.SystemAlerts)
            .HasDefaultValue(true);

        builder.Property(np => np.CartReminders)
            .HasDefaultValue(true);

        // Configure one-to-one relationship with User
        builder.HasOne(np => np.User)
            .WithOne(u => u.NotificationPreference)
            .HasForeignKey<NotificationPreference>(np => np.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(np => np.UserId)
            .IsUnique();
    }
}