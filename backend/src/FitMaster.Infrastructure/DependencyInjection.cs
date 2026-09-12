using System.Net.Http.Headers;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.NutritionGeneration;
using FitMaster.Application.Pdf;
using FitMaster.Application.MembershipProvisioning;
using FitMaster.Infrastructure.Ai;
using FitMaster.Infrastructure.Files;
using FitMaster.Infrastructure.Identity;
using FitMaster.Infrastructure.Payments;
using FitMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        // Identity / Auth
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddSingleton<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // AI meal plan generation (Phase 7) - Groq:ApiKey is User-Secrets-only, never in
        // appsettings.json. Deliberately not validated eagerly here (unlike Jwt:SecretKey):
        // it's only needed for nutrition plan generation, so a missing key shouldn't take
        // down every other module at startup - GroqMealPlanClient checks it lazily instead.
        services.Configure<GroqSettings>(configuration.GetSection("Groq"));
        services.AddHttpClient<IGroqMealPlanClient, GroqMealPlanClient>((provider, client) =>
        {
            var groqSettings = provider.GetRequiredService<IOptions<GroqSettings>>().Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", groqSettings.ApiKey);
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        // File uploads (Phase 9) - same lazy-credential-check pattern as Groq above.
        services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
        services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();

        // Exercise images for the workout plan PDF - short timeout so one slow/dead
        // Cloudinary asset can't stall a PDF export; the fetcher itself never throws.
        services.AddHttpClient<IExerciseImageFetcher, ExerciseImageFetcher>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // Online payments (Stripe) - Stripe:SecretKey / Stripe:WebhookSecret are
        // User-Secrets-only and checked eagerly at startup in Program.cs (unlike Groq
        // above): a missing key here wouldn't just disable one feature, it would leave
        // the webhook endpoint either broken or unable to verify signatures at all.
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
        services.AddScoped<IPaymentGatewayService, StripePaymentGatewayService>();
        services.AddScoped<IMembershipProvisioningService, MembershipProvisioningService>();

        return services;
    }
}
