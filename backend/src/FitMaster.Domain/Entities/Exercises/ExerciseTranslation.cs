using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Exercises;


public class ExerciseTranslation : BaseEntity
{
    public required Guid ExerciseId { get; set; }

    public Exercise Exercise { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Instructions { get; set; }

    public string? AudioCueUrl { get; set; }

   
    public string? Locale { get; set; }
}
