using FitMaster.Domain.Common;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Exercises;

/// <summary>
/// A catalog exercise (e.g. "Barbell Bench Press"). This is read-only reference
/// data seeded once (see the architecture decision in the project plan) - the
/// application never creates/updates/deletes these through its own CRUD flows.
/// </summary>
public class Exercise : BaseEntity<Guid>
{
    /// <summary>Short external code, e.g. from the source dataset.</summary>
    public string? Code { get; set; }

    public DifficultyLevel? DifficultyLevel { get; set; }

    public bool IsArchived { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ExerciseMuscle> ExerciseMuscles { get; set; } = new List<ExerciseMuscle>();

    public ICollection<ExerciseEquipment> ExerciseEquipments { get; set; } = new List<ExerciseEquipment>();

    public ICollection<MovementPattern> MovementPatterns { get; set; } = new List<MovementPattern>();

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public ICollection<ExerciseTranslation> Translations { get; set; } = new List<ExerciseTranslation>();

    public ICollection<ExerciseMedia> Media { get; set; } = new List<ExerciseMedia>();
}
