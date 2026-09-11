using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<MarkAllNotificationsAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var states = await db.NotificationUserStates
            .Where(s => s.UserId == currentUser.UserId && !s.Deleted && !s.Read)
            .ToListAsync(cancellationToken);

        foreach (var state in states)
        {
            state.Read = true;
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
