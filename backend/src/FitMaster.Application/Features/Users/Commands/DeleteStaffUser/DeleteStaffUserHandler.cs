using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Users.Commands.DeleteStaffUser;

public class DeleteStaffUserHandler(IApplicationDbContext db, ICurrentUserService currentUser, IPublisher publisher)
    : IRequestHandler<DeleteStaffUserCommand, Result>
{
    public async Task<Result> Handle(DeleteStaffUserCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.Deleted, cancellationToken);
        if (user is null)
        {
            return Result.Failure("Staff account not found.");
        }

        user.Deleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new ActivityOccurredEvent(
            currentUser.UserId!.Value, ActionType.Delete, EntityType.Employee, user.Id,
            $"Deleted staff account \"{user.FullName}\""), cancellationToken);

        return Result.Success();
    }
}
