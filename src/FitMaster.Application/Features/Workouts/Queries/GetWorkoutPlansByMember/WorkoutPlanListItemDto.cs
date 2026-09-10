using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlansByMember;

public record WorkoutPlanListItemDto(
    long Id,
    string? Name,
    SplitType SplitType,
    WorkoutPlanStatus Status,
    DateTime CreatedAt);
