using FitMaster.Application.Common.Interfaces;
using FitMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitMaster.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found. " +
                "Set it via User Secrets: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\"");

        services.AddDbContext<FitMasterDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<FitMasterDbContext>());

        return services;
    }
}
