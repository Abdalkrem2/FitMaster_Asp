using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Users.Commands.UpdateStaffUser;

public class UpdateStaffUserHandler(IApplicationDbContext db, IPasswordHasher passwordHasher, ICurrentUserService currentUser, IPublisher publisher)
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

        var wasActivated = user.IsActivated;
        var oldRoleName = user.Roles.Select(r => r.RoleName).FirstOrDefault();

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

        var roleChanged = false;
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
            roleChanged = true;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        var changes = new List<string>();
        if (roleChanged) changes.Add($"role → {request.Role}");

        string headline;
        if (wasActivated && request.IsActivated == false)
        {
            headline = $"Deactivated staff account \"{user.FullName}\"";
        }
        else if (!wasActivated && request.IsActivated == true)
        {
            headline = $"Reactivated staff account \"{user.FullName}\"";
        }
        else
        {
            headline = $"Updated staff account \"{user.FullName}\" ({oldRoleName})";
        }
        var details = changes.Count > 0 ? $"{headline} ({string.Join(", ", changes)})" : headline;

        await publisher.Publish(new ActivityOccurredEvent(
            currentUser.UserId!.Value, ActionType.Update, EntityType.Employee, user.Id, details), cancellationToken);

        return Result.Success();
    }
}
