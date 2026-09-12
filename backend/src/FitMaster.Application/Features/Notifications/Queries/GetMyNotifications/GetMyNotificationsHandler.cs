using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetMyNotificationsQuery, NotificationsPageDto>
{
    public async Task<NotificationsPageDto> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = db.NotificationUserStates.Where(s => s.UserId == currentUser.UserId && !s.Deleted);

        var totalElements = await query.CountAsync(cancellationToken);
        var unreadCount = await query.CountAsync(s => !s.Read, cancellationToken);

        var page = Math.Max(request.Page, 0);
        var size = request.Size <= 0 ? 20 : request.Size;
        var totalPages = size <= 0 ? 0 : (int)Math.Ceiling(totalElements / (double)size);

        var content = await query
            .OrderByDescending(s => s.AssignedAt)
            .Skip(page * size)
            .Take(size)
            .Select(s => new NotificationDto(
                s.NotificationId,
                s.Notification.Type,
                s.Notification.Message,
                s.Notification.Details,
                s.Read,
                s.Notification.CreatedAt))
            .ToListAsync(cancellationToken);

        return new NotificationsPageDto(content, page, size, totalElements, totalPages, page >= totalPages - 1, unreadCount);
    }
}
