using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Commands.ChangePassword;

public class ChangePasswordHandler(IApplicationDbContext db, IPasswordHasher passwordHasher)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.Deleted, cancellationToken);
        if (user is null)
        {
            return Result.Failure("User not found.");
        }
        if (!passwordHasher.Verify(user.PasswordHash, request.CurrentPassword))
        {
            return Result.Failure("Current password is incorrect.");
        }

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
