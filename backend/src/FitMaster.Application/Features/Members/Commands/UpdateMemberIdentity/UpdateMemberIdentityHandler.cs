using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Commands.UpdateMemberIdentity;

public class UpdateMemberIdentityHandler(IApplicationDbContext db, ICurrentUserService currentUser, IPublisher publisher)
    : IRequestHandler<UpdateMemberIdentityCommand, Result>
{
    public async Task<Result> Handle(UpdateMemberIdentityCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.MemberId && !u.Deleted, cancellationToken);
        if (user is null)
        {
            return Result.Failure("Member not found.");
        }

        var phoneTaken = await db.Users.AnyAsync(u => u.Phone == request.Phone && u.Id != request.MemberId, cancellationToken);
        if (phoneTaken)
        {
            return Result.Failure("This phone number is already registered.");
        }

        var oldName = user.FullName;

        user.FullName = request.FullName;
        user.Phone = request.Phone;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        var details = oldName == request.FullName
            ? $"Updated contact details for member \"{user.FullName}\""
            : $"Renamed member \"{oldName}\" to \"{user.FullName}\"";
        await publisher.Publish(new ActivityOccurredEvent(
            currentUser.UserId!.Value, ActionType.Update, EntityType.Member, user.Id, details), cancellationToken);

        return Result.Success();
    }
}
