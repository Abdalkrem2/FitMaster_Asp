using FitMaster.Application.Common.Exceptions;
using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Entities.Nutrition;
using FitMaster.Domain.Enums;

namespace FitMaster.Application.NutritionGeneration;

/// <summary>
/// Builds a full <see cref="NutritionPlan"/> (meals + foods + recipe steps) for a
/// member. Calorie/macro targets are computed deterministically by
/// <see cref="ICalorieCalculator"/>; actual meal content comes from an AI chat
/// completion (<see cref="IGroqMealPlanClient"/>), since - unlike Exercise - there is
/// no reference food catalog to assemble meals from. Retries the AI call a few times
/// on a malformed response before giving up. Does not save - the caller
/// (GenerateNutritionPlanHandler) owns persistence.
/// </summary>
public interface INutritionPlanGenerator
{
    Task<NutritionPlan> GenerateAsync(MemberProfile profile, string? gender, CancellationToken cancellationToken);
}

public class NutritionPlanGenerator(
    ICalorieCalculator calorieCalculator,
    INutritionPromptBuilder promptBuilder,
    IGroqMealPlanClient groqClient,
    IMealPlanResponseParser responseParser) : INutritionPlanGenerator
{
    private const int MaxAttempts = 3;

    public async Task<NutritionPlan> GenerateAsync(MemberProfile profile, string? gender, CancellationToken cancellationToken)
    {
        var targets = calorieCalculator.Calculate(profile, gender);
        var prompt = promptBuilder.Build(targets, profile);

        var generated = await GetMealPlanWithRetryAsync(prompt, cancellationToken);

        var plan = new NutritionPlan
        {
            MemberId = profile.MemberId,
            Goal = profile.Goal,
            DailyCalories = targets.Calories,
            ProteinGrams = targets.ProteinGrams,
            CarbsGrams = targets.CarbsGrams,
            FatGrams = targets.FatGrams,
            Status = WorkoutPlanStatus.Active,
            CreatedAt = DateTime.UtcNow,
        };

        foreach (var meal in generated.Meals)
        {
            var nutritionMeal = new NutritionMeal
            {
                NutritionPlanId = 0, // set by EF via the owning collection
                Name = meal.Name,
                MealTime = meal.MealTime,
                PrepTime = meal.PrepTime,
                TotalCalories = meal.Foods.Sum(f => f.Calories),
            };

            foreach (var food in meal.Foods)
            {
                nutritionMeal.Foods.Add(new NutritionFood
                {
                    MealId = 0, // set by EF via the owning collection
                    Name = food.Name,
                    Amount = food.Amount,
                    Calories = food.Calories,
                    ProteinGrams = food.ProteinGrams,
                    CarbsGrams = food.CarbsGrams,
                    FatGrams = food.FatGrams,
                });
            }

            foreach (var step in meal.RecipeSteps)
            {
                nutritionMeal.RecipeSteps.Add(new NutritionRecipeStep
                {
                    MealId = 0, // set by EF via the owning collection
                    StepOrder = step.StepOrder,
                    Instruction = step.Instruction,
                });
            }

            plan.Meals.Add(nutritionMeal);
        }

        // Persist what was actually generated, not just the targets - the AI won't
        // always land exactly on them.
        plan.DailyCalories = plan.Meals.Sum(m => m.TotalCalories);
        plan.ProteinGrams = plan.Meals.SelectMany(m => m.Foods).Sum(f => f.ProteinGrams);
        plan.CarbsGrams = plan.Meals.SelectMany(m => m.Foods).Sum(f => f.CarbsGrams);
        plan.FatGrams = plan.Meals.SelectMany(m => m.Foods).Sum(f => f.FatGrams);

        return plan;
    }

    private async Task<GeneratedMealPlan> GetMealPlanWithRetryAsync(string prompt, CancellationToken cancellationToken)
    {
        Exception? lastError = null;

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                var completion = await groqClient.GetChatCompletionAsync(prompt, cancellationToken);
                return responseParser.Parse(completion);
            }
            catch (Exception ex) when (
                ex is InvalidAiResponseException or HttpRequestException
                || (ex is TaskCanceledException && !cancellationToken.IsCancellationRequested))
            {
                lastError = ex;

                // Brief backoff before retrying - a malformed response can be retried
                // immediately, but transient/rate-limit failures need a moment to clear.
                if (attempt < MaxAttempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                }
            }
        }

        throw new InvalidAiResponseException(
            $"AI meal plan generation failed after {MaxAttempts} attempts: {lastError?.Message}");
    }
}
