using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Commands.UpdateMemberIdentity;

public class UpdateMemberIdentityHandler(IApplicationDbContext db) : IRequestHandler<UpdateMemberIdentityCommand, Result>
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

        user.FullName = request.FullName;
        user.Phone = request.Phone;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
