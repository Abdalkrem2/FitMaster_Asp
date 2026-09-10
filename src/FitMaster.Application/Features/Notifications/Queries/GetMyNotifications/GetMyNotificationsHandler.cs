using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetMyNotificationsQuery, List<NotificationDto>>
{
    public Task<List<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        => db.NotificationUserStates
            .Where(s => s.UserId == currentUser.UserId && !s.Deleted)
            .OrderByDescending(s => s.AssignedAt)
            .Select(s => new NotificationDto(
                s.NotificationId,
                s.Notification.Type,
                s.Notification.Message,
                s.Notification.Details,
                s.Read,
                s.Notification.CreatedAt))
            .ToListAsync(cancellationToken);
}
