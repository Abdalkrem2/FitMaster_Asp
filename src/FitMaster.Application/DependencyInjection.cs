using System.Reflection;
using FitMaster.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FitMaster.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Registers every IRequestHandler<,> found in this assembly (one per
        // Command/Query, added as we build out each Feature in later phases).
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Order matters: logging wraps everything, validation runs before
            // the actual handler.
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Registers every AbstractValidator<T> found in this assembly.
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
