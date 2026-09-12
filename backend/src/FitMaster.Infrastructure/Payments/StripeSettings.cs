namespace FitMaster.Infrastructure.Payments;

public class StripeSettings
{
    /// <summary>User-Secrets-only, never in appsettings.json. Checked eagerly at
    /// startup (Program.cs) - same fail-fast pattern as Jwt:SecretKey.</summary>
    public string SecretKey { get; set; } = "";

    /// <summary>User-Secrets-only, never in appsettings.json. Also checked eagerly
    /// at startup: without it the webhook endpoint can't verify signatures at all,
    /// which would leave it either completely broken or (worse) wide open.</summary>
    public string WebhookSecret { get; set; } = "";

    /// <summary>Not a secret - meant to be exposed to the frontend, so this one
    /// can live in appsettings.json.</summary>
    public string PublishableKey { get; set; } = "";
}
