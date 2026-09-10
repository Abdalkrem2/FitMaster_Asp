using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace FitMaster.Infrastructure.Identity;

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
