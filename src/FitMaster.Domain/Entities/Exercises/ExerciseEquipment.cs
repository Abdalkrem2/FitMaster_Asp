namespace FitMaster.Domain.Entities.Exercises;

/// <summary>
/// Join entity: which equipment an exercise needs and whether it's required
/// or optional. Composite key (ExerciseId, EquipmentId) - configured in Infrastructure.
/// </summary>
public class ExerciseEquipment
{
    public required Guid ExerciseId { get; set; }

    public Exercise Exercise { get; set; } = null!;

    public required long EquipmentId { get; set; }

    public Equipment Equipment { get; set; } = null!;

    public bool IsRequired { get; set; }
}
