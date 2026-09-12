using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Nutrition;

/// <summary>A single ordered preparation step for a NutritionMeal.</summary>
public class NutritionRecipeStep : BaseEntity
{
    public required long MealId { get; set; }

    public NutritionMeal Meal { get; set; } = null!;

    public required int StepOrder { get; set; }

    public required string Instruction { get; set; }
}
