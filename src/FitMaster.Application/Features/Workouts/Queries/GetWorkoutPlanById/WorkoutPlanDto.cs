using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;

public record WorkoutPlanDto(
    long Id,
    string? Name,
    long MemberId,
    FitnessGoal Goal,
    FitnessLevel Level,
    SplitType SplitType,
    WorkoutPlanStatus Status,
    DateTime CreatedAt,
    IReadOnlyList<WorkoutDayDto> WorkoutDays);

public record WorkoutDayDto(
    long Id,
    int DayNumber,
    string MuscleGroupLabel,
    IReadOnlyList<WorkoutExerciseDto> WorkoutExercises);

public record WorkoutExerciseDto(
    long Id,
    Guid ExerciseId,
    string? ExerciseName,
    int OrderIndex,
    int? Sets,
    int? Reps,
    int? RepsMax,
    int? DurationSeconds);
