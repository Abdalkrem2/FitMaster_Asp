namespace FitMaster.Application.Common.Interfaces;

/// <summary>
/// Exposes who the current HTTP request is authenticated as (read from the JWT
/// claims). Implemented in Infrastructure via IHttpContextAccessor - Handlers
/// depend on this instead of touching HttpContext directly.
/// </summary>
public interface ICurrentUserService
{
    long? UserId { get; }

    bool IsAuthenticated { get; }
}
