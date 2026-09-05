using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Workouts;

/// <summary>A generated (or manually built) multi-day workout plan for a member.</summary>
public class WorkoutPlan : BaseEntity
{
    public string? Name { get; set; }

    public required FitnessGoal Goal { get; set; }

    public required FitnessLevel Level { get; set; }

    public WorkoutPlanStatus Status { get; set; } = WorkoutPlanStatus.Active;

    public required SplitType SplitType { get; set; }

    public required long MemberId { get; set; }

    public User Member { get; set; } = null!;

    public ICollection<WorkoutDay> WorkoutDays { get; set; } = new List<WorkoutDay>();

    public DateTime CreatedAt { get; set; }
}
