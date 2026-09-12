using System.Reflection;
using FitMaster.Application.Common.Behaviors;
using FitMaster.Application.NutritionGeneration;
using FitMaster.Application.Pdf;
using FitMaster.Application.WorkoutGeneration;
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

        // Workout generation - rule-based, independently testable components (Phase 6).
        services.AddScoped<IDaySplitter, DaySplitter>();
        services.AddScoped<IExerciseSelector, ExerciseSelector>();
        services.AddScoped<IVolumePrescriber, VolumePrescriber>();
        services.AddScoped<IWorkoutPlanGenerator, WorkoutPlanGenerator>();

        // Nutrition generation (Phase 7) - deterministic calorie/macro calc + AI meal
        // content (IGroqMealPlanClient is implemented in Infrastructure).
        services.AddScoped<ICalorieCalculator, CalorieCalculator>();
        services.AddScoped<INutritionPromptBuilder, NutritionPromptBuilder>();
        services.AddScoped<IMealPlanResponseParser, MealPlanResponseParser>();
        services.AddScoped<INutritionPlanGenerator, NutritionPlanGenerator>();

        // PDF export (frontend integration pass) - QuestPDF Community license, see Program.cs.
        services.AddScoped<IWorkoutPlanPdfGenerator, WorkoutPlanPdfGenerator>();
        services.AddScoped<INutritionPlanPdfGenerator, NutritionPlanPdfGenerator>();

        return services;
    }
}
