using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Nutrition;

/// <summary>A single meal (e.g. "Breakfast") within a NutritionPlan.</summary>
public class NutritionMeal : BaseEntity
{
    public required long NutritionPlanId { get; set; }

    public NutritionPlan NutritionPlan { get; set; } = null!;

    public required string Name { get; set; }

    /// <summary>Free-text time slot, e.g. "8:00 AM".</summary>
    public required string MealTime { get; set; }

    public string? PrepTime { get; set; }

    public required int TotalCalories { get; set; }

    public ICollection<NutritionFood> Foods { get; set; } = new List<NutritionFood>();

    public ICollection<NutritionRecipeStep> RecipeSteps { get; set; } = new List<NutritionRecipeStep>();
}
