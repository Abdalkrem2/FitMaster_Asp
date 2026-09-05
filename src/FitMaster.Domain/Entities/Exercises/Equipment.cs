using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Exercises;

/// <summary>A piece of equipment an exercise may require. Reference data.</summary>
public class Equipment : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<ExerciseEquipment> ExerciseEquipments { get; set; } = new List<ExerciseEquipment>();
}
