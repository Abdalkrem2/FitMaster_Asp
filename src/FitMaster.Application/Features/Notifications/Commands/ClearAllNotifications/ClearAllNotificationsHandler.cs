using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Notifications.Commands.ClearAllNotifications;

public class ClearAllNotificationsHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<ClearAllNotificationsCommand, Result>
{
    public async Task<Result> Handle(ClearAllNotificationsCommand request, CancellationToken cancellationToken)
    {
        var states = await db.NotificationUserStates
            .Where(s => s.UserId == currentUser.UserId && !s.Deleted)
            .ToListAsync(cancellationToken);

        foreach (var state in states)
        {
            state.Deleted = true;
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
