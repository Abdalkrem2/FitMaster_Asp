using FitMaster.Domain.Common;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Exercises;

/// <summary>A muscle that an exercise can target. Reference data - read-only at runtime.</summary>
public class Muscle : BaseEntity
{
    public required string Name { get; set; }

    public required BodyRegion BodyRegion { get; set; }

    public ICollection<ExerciseMuscle> ExerciseMuscles { get; set; } = new List<ExerciseMuscle>();
}
