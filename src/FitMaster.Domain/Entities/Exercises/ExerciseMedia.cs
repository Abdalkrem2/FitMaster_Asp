using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Exercises;

/// <summary>Links an Exercise to a MediaAsset (e.g. its demonstration video for a given angle/sex).</summary>
public class ExerciseMedia : BaseEntity
{
    public required Guid ExerciseId { get; set; }

    public Exercise Exercise { get; set; } = null!;

    public required Guid MediaAssetId { get; set; }

    public MediaAsset MediaAsset { get; set; } = null!;

    public string? Role { get; set; }

    public string? Sex { get; set; }

    public string? ViewAngle { get; set; }
}
