using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.ActivityLogging;
using FitMaster.Domain.Entities.Notifications;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.ActivityLogging;

/// <summary>
/// Writes an ActivityLog row for each domain event, then fans a Notification out
/// to every Admin - the only recipient scope requested for this activity feed.
/// </summary>
public class ActivityLogEventHandler(IApplicationDbContext db) :
    INotificationHandler<MemberCreatedEvent>,
    INotificationHandler<StaffUserCreatedEvent>,
    INotificationHandler<MembershipCreatedEvent>,
    INotificationHandler<MembershipRenewedEvent>
{
    public Task Handle(MemberCreatedEvent notification, CancellationToken cancellationToken)
        => LogAndNotifyAsync(notification.PerformedById, ActionType.Create, EntityType.Member, notification.MemberId,
            "A new member was created.", cancellationToken);

    public Task Handle(StaffUserCreatedEvent notification, CancellationToken cancellationToken)
        => LogAndNotifyAsync(notification.PerformedById, ActionType.Create, EntityType.Employee, notification.UserId,
            "A new staff account was created.", cancellationToken);

    public Task Handle(MembershipCreatedEvent notification, CancellationToken cancellationToken)
        => LogAndNotifyAsync(notification.PerformedById, ActionType.Create, EntityType.Membership, notification.MembershipId,
            "A new membership was created.", cancellationToken);

    public Task Handle(MembershipRenewedEvent notification, CancellationToken cancellationToken)
        => LogAndNotifyAsync(notification.PerformedById, ActionType.Renew, EntityType.Membership, notification.MembershipId,
            "A membership was renewed.", cancellationToken);

    private async Task LogAndNotifyAsync(
        long performedById, ActionType action, EntityType entityType, long entityId, string message, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var log = new ActivityLog
        {
            PerformedById = performedById,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            CreatedAt = now,
        };
        db.ActivityLogs.Add(log);
        await db.SaveChangesAsync(cancellationToken);

        var adminIds = await db.Users
            .Where(u => !u.Deleted && u.Roles.Any(r => r.RoleName == AppRole.Admin))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);
        if (adminIds.Count == 0) return;

        var recipientNotification = new Notification
        {
            Type = NotificationType.ActivityLog,
            ReferenceId = log.Id,
            Message = message,
            CreatedAt = now,
        };
        foreach (var adminId in adminIds)
        {
            recipientNotification.UserStates.Add(new NotificationUserState
            {
                NotificationId = 0, // set by EF via the owning collection
                UserId = adminId,
                AssignedAt = now,
            });
        }
        db.Notifications.Add(recipientNotification);
        await db.SaveChangesAsync(cancellationToken);
    }
}
