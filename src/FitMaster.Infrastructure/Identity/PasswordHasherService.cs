using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace FitMaster.Infrastructure.Identity;

/// <summary>
/// Wraps ASP.NET Core Identity's <see cref="PasswordHasher{TUser}"/> (PBKDF2 with
/// HMAC-SHA256) behind our own interface, so the rest of the app never depends
/// on Identity types directly. The generic parameter isn't actually used by the
/// default hashing algorithm, so passing null for it is safe.
/// </summary>
public class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
