using FitMaster.Domain.Common;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Exercises;


public class Exercise : BaseEntity<Guid>
{
   
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
