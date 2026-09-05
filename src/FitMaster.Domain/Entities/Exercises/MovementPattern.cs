using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Exercises;

/// <summary>A fundamental movement pattern (e.g. "Squat", "Hinge", "Push"). Reference data.</summary>
public class MovementPattern : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}
