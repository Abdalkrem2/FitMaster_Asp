using System.Text.Json;
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
    DifficultyLevel? Difficulty,
    string? PrimaryMuscle,
    IReadOnlyList<string> TargetMuscles,
    IReadOnlyList<string> Equipment,
    string? ImageUrl,
    int OrderIndex,
    int? Sets,
    int? Reps,
    int? RepsMax,
    int? DurationSeconds,
    IReadOnlyList<string> Instructions);

/// <summary>
/// ExerciseTranslation.Instructions is stored as a JSON-array string (e.g.
/// '["Step one.", "Step two."]') - EF can't translate JsonSerializer calls inside
/// a query, so every handler that builds a WorkoutExerciseDto fetches the raw
/// string as a plain column and parses it here, after materializing.
/// </summary>
public static class WorkoutInstructionsParser
{
    public static IReadOnlyList<string> Parse(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson)) return [];
        try
        {
            return JsonSerializer.Deserialize<List<string>>(rawJson) ?? [];
        }
        catch (JsonException)
        {
            // Not valid JSON - show the raw text as a single line rather than
            // silently dropping it or crashing the whole plan fetch.
            return [rawJson];
        }
    }
}
