namespace FitMaster.Application.WorkoutGeneration;

/// <summary>
/// Name keywords for elite gymnastic-strength skills and Olympic-lift derivatives that
/// don't belong in an auto-generated general fitness/hypertrophy/strength plan for any
/// member level - a stopgap for the seeded exercise catalog currently classifying every
/// exercise as DifficultyLevel=Intermediate, so <see cref="ExerciseSelector"/>'s difficulty
/// filter alone can't tell "planche push-up" from "push-up". Matched case-insensitively
/// against the exercise's English translation name.
///
/// Built from querying the actual seeded exercise_translations data (1,324 exercises), not
/// guessed from memory. That audit found the previous "one arm"/"one-arm" keywords matched
/// 117 of 1,180 strength exercises (~10% of the catalog) - almost all of them completely
/// ordinary unilateral dumbbell/cable/machine work ("dumbbell one arm lateral raise",
/// "cable one arm curl", "band one arm overhead biceps curl", ...), not elite skills. Those
/// were replaced with exact phrases for the handful of genuinely elite bodyweight variants
/// that actually exist in the data ("one arm chin-up", "one arm dip", "one hand pull up").
/// Keep new entries this specific - a keyword here should only ever match a real
/// years-of-dedicated-training skill move, never a normal loaded exercise.
/// </summary>
public static class AdvancedMovementBlocklist
{
    public static readonly IReadOnlyList<string> Keywords =
    [
        "planche",
        "maltese",
        "muscle up",
        "muscle-up",
        "handstand",
        "human flag",
        "front lever",
        "back lever",
        "iron cross",
        "one arm chin-up",
        "one arm chin up",
        "one-arm chin-up",
        "one arm dip",
        "one-arm dip",
        "one hand pull up",
        "one hand pull-up",
        "one-hand pull-up",
        "pistol squat",
        "dragon flag",
        "l-sit",
        "archer",
        "kipping",
        "snatch",
        "jerk", // covers "clean and jerk" and its kettlebell/dumbbell/squat-jerk variants too
    ];

    public static bool IsAdvancedMovement(string? exerciseName)
        => !string.IsNullOrEmpty(exerciseName)
        && Keywords.Any(keyword => exerciseName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
}
