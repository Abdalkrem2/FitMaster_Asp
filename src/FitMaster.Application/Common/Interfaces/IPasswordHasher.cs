namespace FitMaster.Application.Common.Interfaces;

/// <summary>Hashes and verifies passwords. Implemented in Infrastructure via ASP.NET Core Identity's PasswordHasher.</summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string hashedPassword, string providedPassword);
}
