using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Auth.Commands.Register;

public class RegisterHandler(
    IApplicationDbContext db,
    IPasswordHasher passwordHasher,
    IJwtService jwtService)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var phoneTaken = await db.Users.AnyAsync(u => u.Phone == request.Phone, cancellationToken);
        if (phoneTaken)
        {
            return Result<RegisterResponse>.Failure("This phone number is already registered.");
        }

        // Every new self-registered account starts as a plain Member. Promoting
        // someone to Employee/Admin is a separate, privileged action (later phase).
        var memberRole = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == AppRole.Member, cancellationToken);
        if (memberRole is null)
        {
            memberRole = new Role { RoleName = AppRole.Member };
            db.Roles.Add(memberRole);
        }

        var user = new User
        {
            Phone = request.Phone,
            PasswordHash = passwordHasher.Hash(request.Password),
            FullName = request.FullName,
            IsActivated = true,
            Deleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Roles = [memberRole],
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(user);

        return Result<RegisterResponse>.Success(
            new RegisterResponse(user.Id, user.FullName, token.Token, token.ExpiresAtUtc));
    }
}
