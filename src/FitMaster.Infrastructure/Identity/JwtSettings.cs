namespace FitMaster.Infrastructure.Identity;

/// <summary>
/// Bound from the "Jwt" configuration section. Issuer/Audience/ExpiryMinutes
/// live in appsettings.json (not secret); SecretKey must be set via User
/// Secrets (`dotnet user-secrets set "Jwt:SecretKey" "..."`) and never committed.
/// </summary>
public class JwtSettings
{
    public required string SecretKey { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public int ExpiryMinutes { get; set; } = 120;
}
