using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Exercises;

namespace FitMaster.Domain.Entities.Workouts;

/// <summary>
/// One prescribed exercise within a WorkoutDay: which catalog Exercise, in what
/// order, and with what sets/reps (or duration, for cardio-style exercises).
/// </summary>
public class WorkoutExercise : BaseEntity
{
    public required long WorkoutDayId { get; set; }

    public WorkoutDay WorkoutDay { get; set; } = null!;

    public required Guid ExerciseId { get; set; }

    public Exercise Exercise { get; set; } = null!;

    /// <summary>Position of this exercise within the day (1, 2, 3...).</summary>
    public required int OrderIndex { get; set; }

    public int? Sets { get; set; }

    public int? Reps { get; set; }

    /// <summary>Upper bound of the rep range, when reps are prescribed as a range.</summary>
    public int? RepsMax { get; set; }

    /// <summary>Used instead of Reps for cardio/timed exercises.</summary>
    public int? DurationSeconds { get; set; }
}
