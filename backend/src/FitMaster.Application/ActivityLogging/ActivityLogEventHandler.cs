using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.ActivityLogging;
using FitMaster.Domain.Entities.Notifications;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.ActivityLogging;

/// <summary>
/// Writes an ActivityLog row for the event, then fans a Notification out to
/// every Admin - the only recipient scope requested for this activity feed.
/// Reuses the same Details text for the notification message so admins see the
/// same specific, human-readable description in both places.
/// </summary>
public class ActivityLogEventHandler(IApplicationDbContext db) : INotificationHandler<ActivityOccurredEvent>
{
    public async Task Handle(ActivityOccurredEvent notification, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var log = new ActivityLog
        {
            PerformedById = notification.PerformedById,
            Action = notification.Action,
            EntityType = notification.EntityType,
            EntityId = notification.EntityId,
            Details = notification.Details,
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
            Message = notification.Details,
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
