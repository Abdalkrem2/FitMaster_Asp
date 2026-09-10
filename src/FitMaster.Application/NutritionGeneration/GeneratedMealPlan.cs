namespace FitMaster.Application.NutritionGeneration;

/// <summary>Plain-data shape of an AI-generated meal plan, decoupled from the persisted entities.</summary>
public record GeneratedMealPlan(IReadOnlyList<GeneratedMeal> Meals);

public record GeneratedMeal(
    string Name,
    string MealTime,
    string? PrepTime,
    IReadOnlyList<GeneratedFood> Foods,
    IReadOnlyList<GeneratedRecipeStep> RecipeSteps);

public record GeneratedFood(string Name, string Amount, int Calories, int ProteinGrams, int CarbsGrams, int FatGrams);

public record GeneratedRecipeStep(int StepOrder, string Instruction);
