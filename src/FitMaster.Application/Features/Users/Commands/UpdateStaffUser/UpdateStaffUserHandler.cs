using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Users.Commands.UpdateStaffUser;

public class UpdateStaffUserHandler(IApplicationDbContext db, IPasswordHasher passwordHasher)
    : IRequestHandler<UpdateStaffUserCommand, Result>
{
    public async Task<Result> Handle(UpdateStaffUserCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.Deleted, cancellationToken);
        if (user is null)
        {
            return Result.Failure("Staff account not found.");
        }

        var phoneTaken = await db.Users.AnyAsync(u => u.Phone == request.Phone && u.Id != request.UserId, cancellationToken);
        if (phoneTaken)
        {
            return Result.Failure("This phone number is already registered.");
        }

        user.FullName = request.FullName;
        user.Phone = request.Phone;
        user.Gender = request.Gender;

        if (request.Password is not null)
        {
            user.PasswordHash = passwordHasher.Hash(request.Password);
        }

        if (request.IsActivated is not null)
        {
            user.IsActivated = request.IsActivated.Value;
        }

        if (request.Role is not null && user.Roles.All(r => r.RoleName != request.Role))
        {
            var role = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == request.Role, cancellationToken);
            if (role is null)
            {
                role = new Role { RoleName = request.Role.Value };
                db.Roles.Add(role);
            }

            user.Roles.Clear();
            user.Roles.Add(role);
        }

        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
