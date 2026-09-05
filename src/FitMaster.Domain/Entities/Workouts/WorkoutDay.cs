using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Workouts;

/// <summary>A single training day within a WorkoutPlan (e.g. "Push Day").</summary>
public class WorkoutDay : BaseEntity
{
    public required long WorkoutPlanId { get; set; }

    public WorkoutPlan WorkoutPlan { get; set; } = null!;

    /// <summary>Human-readable label, e.g. "Chest & Triceps".</summary>
    public required string MuscleGroupLabel { get; set; }

    public required int DayNumber { get; set; }

    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}
