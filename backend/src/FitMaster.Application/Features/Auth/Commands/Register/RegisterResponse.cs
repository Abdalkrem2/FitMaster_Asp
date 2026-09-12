namespace FitMaster.Application.Features.Auth.Commands.Register;

/// <summary>Returned on successful registration - the new user is logged in immediately.</summary>
public record RegisterResponse(long UserId, string FullName, string Token, DateTime ExpiresAtUtc);
