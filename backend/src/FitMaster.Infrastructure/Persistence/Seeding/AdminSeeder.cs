using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Enums;
using FitMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitMaster.Infrastructure.Persistence.Seeding;

/// <summary>
/// Solves the classic "who creates the first Admin?" bootstrap problem: on
/// every startup, checks whether any Admin exists yet. If not, creates one
/// from configuration - and deliberately has NO hardcoded fallback
/// credentials, so there's never a predictable default admin account sitting
/// in the code. Idempotent: does nothing once an Admin already exists.
/// </summary>
public static class AdminSeeder
{
    public static async Task SeedAdminUserAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FitMasterDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var adminExists = await db.Users
            .AnyAsync(u => u.Roles.Any(r => r.RoleName == AppRole.Admin));

        if (adminExists)
        {
            return;
        }

        var phone = configuration["AdminSeed:Phone"];
        var password = configuration["AdminSeed:Password"];
        var fullName = configuration["AdminSeed:FullName"] ?? "System Administrator";

        if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No Admin account exists yet, and 'AdminSeed:Phone' / 'AdminSeed:Password' are not " +
                "configured. Set them via User Secrets before first run, e.g.:\n" +
                "  dotnet user-secrets set \"AdminSeed:Phone\" \"0700000000\"\n" +
                "  dotnet user-secrets set \"AdminSeed:Password\" \"<a strong password>\"");
        }

        var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == AppRole.Admin);
        if (adminRole is null)
        {
            adminRole = new Role { RoleName = AppRole.Admin };
            db.Roles.Add(adminRole);
        }

        var admin = new User
        {
            Phone = phone,
            PasswordHash = passwordHasher.Hash(password),
            FullName = fullName,
            IsActivated = true,
            Deleted = false,
            AdminRoleAssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Roles = [adminRole],
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
