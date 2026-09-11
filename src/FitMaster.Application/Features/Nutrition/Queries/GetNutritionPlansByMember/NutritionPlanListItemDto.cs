using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlansByMember;

public record NutritionPlanListItemDto(
    long Id,
    FitnessGoal Goal,
    int DailyCalories,
    WorkoutPlanStatus Status,
    DateTime CreatedAt);
