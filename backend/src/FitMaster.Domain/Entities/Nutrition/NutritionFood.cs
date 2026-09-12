using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Nutrition;


public class NutritionFood : BaseEntity
{
    public required long MealId { get; set; }

    public NutritionMeal Meal { get; set; } = null!;


    public required string Name { get; set; }


    public required string Amount { get; set; }

    public required int Calories { get; set; }

    public required int ProteinGrams { get; set; }

    public required int CarbsGrams { get; set; }

    public required int FatGrams { get; set; }
}
