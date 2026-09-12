using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Exercises;


public class Equipment : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<ExerciseEquipment> ExerciseEquipments { get; set; } = new List<ExerciseEquipment>();
}
