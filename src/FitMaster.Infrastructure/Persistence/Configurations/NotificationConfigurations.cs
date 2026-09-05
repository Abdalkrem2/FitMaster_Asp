using FitMaster.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitMaster.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.Property(n => n.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(n => n.ReferenceId).HasColumnName("reference_id");
        builder.Property(n => n.Message).IsRequired();
        builder.Property(n => n.Details);
        builder.Property(n => n.CreatedAt).HasColumnName("created_at").IsRequired();
    }
}

public class NotificationUserStateConfiguration : IEntityTypeConfiguration<NotificationUserState>
{
    public void Configure(EntityTypeBuilder<NotificationUserState> builder)
    {
        builder.ToTable("notification_user_state");

        builder.HasIndex(s => new { s.NotificationId, s.UserId }).IsUnique();

        builder.Property(s => s.NotificationId).HasColumnName("notification_id").IsRequired();
        builder.HasOne(s => s.Notification).WithMany(n => n.UserStates)
            .HasForeignKey(s => s.NotificationId).OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.UserId).HasColumnName("user_id").IsRequired();
        builder.HasOne(s => s.User).WithMany(u => u.NotificationStates)
            .HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(s => s.Read).HasColumnName("is_read").IsRequired();
        builder.Property(s => s.Deleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(s => s.AssignedAt).HasColumnName("assigned_at").IsRequired();
    }
}
