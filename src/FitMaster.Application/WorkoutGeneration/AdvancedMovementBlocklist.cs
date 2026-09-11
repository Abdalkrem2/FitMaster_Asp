namespace FitMaster.Application.WorkoutGeneration;

/// <summary>
/// Name keywords for clearly-advanced skill movements (gymnastic strength skills,
/// Olympic-lift derivatives) that Beginner members shouldn't be prescribed - a stopgap
/// for the seeded exercise catalog currently classifying every exercise as
/// DifficultyLevel=Intermediate, so <see cref="ExerciseSelector"/>'s difficulty filter
/// alone can't tell "planche push-up" from "push-up". Matched case-insensitively
/// against the exercise's English translation name.
/// </summary>
public static class AdvancedMovementBlocklist
{
    public static readonly IReadOnlyList<string> Keywords =
    [
        "planche",
        "muscle up",
        "muscle-up",
        "handstand",
        "human flag",
        "front lever",
        "back lever",
        "iron cross",
        "one arm",
        "one-arm",
        "pistol squat",
        "dragon flag",
        "l-sit",
        "archer",
        "kipping",
        "snatch",
        "clean and jerk",
        "clean & jerk",
    ];

    public static bool IsAdvancedMovement(string? exerciseName)
        => !string.IsNullOrEmpty(exerciseName)
        && Keywords.Any(keyword => exerciseName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
}
