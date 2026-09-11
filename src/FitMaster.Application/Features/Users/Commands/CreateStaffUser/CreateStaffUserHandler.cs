using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Users.Commands.CreateStaffUser;

public class CreateStaffUserHandler(IApplicationDbContext db, IPasswordHasher passwordHasher, ICurrentUserService currentUser, IPublisher publisher)
    : IRequestHandler<CreateStaffUserCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateStaffUserCommand request, CancellationToken cancellationToken)
    {
        var phoneTaken = await db.Users.AnyAsync(u => u.Phone == request.Phone, cancellationToken);
        if (phoneTaken)
        {
            return Result<long>.Failure("This phone number is already registered.");
        }

        var role = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == request.Role, cancellationToken);
        if (role is null)
        {
            role = new Role { RoleName = request.Role };
            db.Roles.Add(role);
        }

        var user = new User
        {
            Phone = request.Phone,
            PasswordHash = passwordHasher.Hash(request.Password),
            FullName = request.FullName,
            Gender = request.Gender,
            IsActivated = true,
            Deleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Roles = [role],
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new StaffUserCreatedEvent(user.Id, currentUser.UserId!.Value), cancellationToken);

        return Result<long>.Success(user.Id);
    }
}
