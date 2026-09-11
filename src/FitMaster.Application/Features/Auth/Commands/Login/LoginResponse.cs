using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Auth.Commands.Login;

public record LoginResponse(
    long UserId,
    string FullName,
    IReadOnlyList<AppRole> Roles,
    string Token,
    DateTime ExpiresAtUtc);
