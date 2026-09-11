using FitMaster.Domain.Entities.Exercises;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Workouts;

/// <summary>
/// Reference data: which muscles to avoid loading for a member-reported injury.
/// Composite key (InjuryType, MuscleId) - configured in Infrastructure, not here.
/// </summary>
public class InjuryMuscleExclusion
{
    public required InjuryType InjuryType { get; set; }

    public required long MuscleId { get; set; }

    public Muscle Muscle { get; set; } = null!;
}
