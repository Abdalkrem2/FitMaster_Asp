using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Commands.DeleteMember;

public class DeleteMemberHandler(IApplicationDbContext db) : IRequestHandler<DeleteMemberCommand, Result>
{
    public async Task<Result> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await db.Users.FirstOrDefaultAsync(u => u.Id == request.MemberId && !u.Deleted, cancellationToken);
        if (member is null)
        {
            return Result.Failure("Member not found.");
        }

        member.Deleted = true;
        member.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
