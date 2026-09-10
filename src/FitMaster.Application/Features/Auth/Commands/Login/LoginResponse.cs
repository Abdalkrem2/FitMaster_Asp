namespace FitMaster.Application.Features.Auth.Commands.Login;

public record LoginResponse(
    long UserId,
    string FullName,
    IReadOnlyList<string> Roles,
    string Token,
    DateTime ExpiresAtUtc);
