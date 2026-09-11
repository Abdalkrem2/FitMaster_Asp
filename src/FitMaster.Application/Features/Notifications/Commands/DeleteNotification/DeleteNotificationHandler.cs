using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Notifications.Commands.DeleteNotification;

public class DeleteNotificationHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteNotificationCommand, Result>
{
    public async Task<Result> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var state = await db.NotificationUserStates
            .FirstOrDefaultAsync(s => s.NotificationId == request.NotificationId && s.UserId == currentUser.UserId, cancellationToken);
        if (state is null)
        {
            return Result.Failure("Notification not found.");
        }

        state.Deleted = true;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
