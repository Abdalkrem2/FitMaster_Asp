using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlanById;

public record NutritionPlanDto(
    long Id,
    long MemberId,
    FitnessGoal Goal,
    int DailyCalories,
    int ProteinGrams,
    int CarbsGrams,
    int FatGrams,
    WorkoutPlanStatus Status,
    DateTime CreatedAt,
    IReadOnlyList<NutritionMealDto> Meals);

public record NutritionMealDto(
    long Id,
    string Name,
    string MealTime,
    string? PrepTime,
    int TotalCalories,
    IReadOnlyList<NutritionFoodDto> Foods,
    IReadOnlyList<NutritionRecipeStepDto> RecipeSteps);

public record NutritionFoodDto(long Id, string Name, string Amount, int Calories, int ProteinGrams, int CarbsGrams, int FatGrams);

public record NutritionRecipeStepDto(long Id, int StepOrder, string Instruction);
