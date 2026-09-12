using FitMaster.Domain.Entities.Identity;

namespace FitMaster.Application.Common.Interfaces;

/// <summary>Generates signed JWT access tokens. Implemented in Infrastructure.</summary>
public interface IJwtService
{
    AuthToken GenerateToken(User user);
}

/// <summary>A generated access token and when it expires (UTC).</summary>
public record AuthToken(string Token, DateTime ExpiresAtUtc);
