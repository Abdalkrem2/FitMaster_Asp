using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Nutrition;

/// <summary>A generated (or manually built) daily nutrition plan for a member.</summary>
public class NutritionPlan : BaseEntity
{
    public required long MemberId { get; set; }

    public User Member { get; set; } = null!;

    public required FitnessGoal Goal { get; set; }

    public required int DailyCalories { get; set; }

    public required int ProteinGrams { get; set; }

    public required int CarbsGrams { get; set; }

    public required int FatGrams { get; set; }

    public WorkoutPlanStatus Status { get; set; } = WorkoutPlanStatus.Active;

    public ICollection<NutritionMeal> Meals { get; set; } = new List<NutritionMeal>();

    public DateTime CreatedAt { get; set; }
}
