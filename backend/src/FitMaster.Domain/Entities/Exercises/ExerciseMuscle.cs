using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Exercises;

/// <summary>
/// Join entity: which muscle an exercise targets and whether it's the primary
/// or secondary mover. Composite key (ExerciseId, MuscleId) - configured in
/// Infrastructure, not here.
/// </summary>
public class ExerciseMuscle
{
    public required Guid ExerciseId { get; set; }

    public Exercise Exercise { get; set; } = null!;

    public required long MuscleId { get; set; }

    public Muscle Muscle { get; set; } = null!;

    public required MuscleRole Role { get; set; }
}
