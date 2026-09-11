using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FitMaster.Application.Common.Exceptions;

namespace FitMaster.Application.NutritionGeneration;

/// <summary>
/// Parses the AI client's raw chat completion text into a <see cref="GeneratedMealPlan"/>,
/// tolerating markdown code fences around the JSON. Pure/data-only - no I/O - so it's
/// unit-testable with a hand-written response string instead of a live AI call.
/// </summary>
public interface IMealPlanResponseParser
{
    GeneratedMealPlan Parse(string rawResponse);
}

public partial class MealPlanResponseParser : IMealPlanResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public GeneratedMealPlan Parse(string rawResponse)
    {
        var json = StripMarkdownFences(rawResponse);

        MealPlanJson? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<MealPlanJson>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidAiResponseException($"AI response was not valid JSON: {ex.Message}");
        }

        if (parsed?.Meals is not { Count: >= 3 } meals)
        {
            throw new InvalidAiResponseException("AI response did not contain at least 3 meals.");
        }
        if (meals.Any(m => string.IsNullOrWhiteSpace(m.Name) || m.Foods is not { Count: >= 1 }))
        {
            throw new InvalidAiResponseException("AI response contained a meal with no name or no foods.");
        }

        return new GeneratedMealPlan(meals
            .Select(m => new GeneratedMeal(
                m.Name!,
                m.MealTime ?? "",
                m.PrepTime,
                (m.Foods ?? []).Select(f => new GeneratedFood(
                    f.Name ?? "food",
                    f.Amount ?? "",
                    f.Calories,
                    f.ProteinGrams,
                    f.CarbsGrams,
                    f.FatGrams)).ToList(),
                (m.RecipeSteps ?? []).Select(s => new GeneratedRecipeStep(s.StepOrder, s.Instruction ?? "")).ToList()))
            .ToList());
    }

    private static string StripMarkdownFences(string response)
        => MarkdownFenceRegex().Replace(response, "").Trim();

    [GeneratedRegex("```json|```", RegexOptions.IgnoreCase)]
    private static partial Regex MarkdownFenceRegex();

    private record MealPlanJson([property: JsonPropertyName("meals")] List<MealJson>? Meals);

    private record MealJson(
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("mealTime")] string? MealTime,
        [property: JsonPropertyName("prepTime")] string? PrepTime,
        [property: JsonPropertyName("foods")] List<FoodJson>? Foods,
        [property: JsonPropertyName("recipeSteps")] List<RecipeStepJson>? RecipeSteps);

    private record FoodJson(
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("amount")] string? Amount,
        [property: JsonPropertyName("calories")] int Calories,
        [property: JsonPropertyName("proteinGrams")] int ProteinGrams,
        [property: JsonPropertyName("carbsGrams")] int CarbsGrams,
        [property: JsonPropertyName("fatGrams")] int FatGrams);

    private record RecipeStepJson(
        [property: JsonPropertyName("stepOrder")] int StepOrder,
        [property: JsonPropertyName("instruction")] string? Instruction);
}
