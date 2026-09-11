using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Users.Commands.DeleteStaffUser;

public class DeleteStaffUserHandler(IApplicationDbContext db) : IRequestHandler<DeleteStaffUserCommand, Result>
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

        return Result.Success();
    }
}
