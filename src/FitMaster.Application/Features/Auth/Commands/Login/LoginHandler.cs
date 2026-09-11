using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Auth.Commands.Login;

public class LoginHandler(
    IApplicationDbContext db,
    IPasswordHasher passwordHasher,
    IJwtService jwtService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    // Deliberately generic message for both "no such phone" and "wrong password" -
    // revealing which one it was makes it easier to enumerate valid phone numbers.
    private const string InvalidCredentialsMessage = "Invalid phone number or password.";

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Phone == request.Phone && !u.Deleted, cancellationToken);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            return Result<LoginResponse>.Failure(InvalidCredentialsMessage);
        }

        var token = jwtService.GenerateToken(user);
        var roles = user.Roles.Select(r => r.RoleName).ToList();

        return Result<LoginResponse>.Success(
            new LoginResponse(user.Id, user.FullName, roles, token.Token, token.ExpiresAtUtc));
    }
}
