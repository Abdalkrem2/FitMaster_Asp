using FitMaster.Domain.Common;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Exercises;

/// <summary>A classification tag for an exercise (category or required-equipment marker). Reference data.</summary>
public class Tag : BaseEntity
{
    public required TagCategory Category { get; set; }

    public required string Name { get; set; }

    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}
