using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var state = await db.NotificationUserStates
            .FirstOrDefaultAsync(s => s.NotificationId == request.NotificationId && s.UserId == currentUser.UserId, cancellationToken);
        if (state is null)
        {
            return Result.Failure("Notification not found.");
        }

        state.Read = true;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
