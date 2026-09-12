using FitMaster.Domain.Common;

namespace FitMaster.Domain.Entities.Exercises;

/// <summary>A single media file (image/video/gif) usable by one or more exercises. Reference data.</summary>
public class MediaAsset : BaseEntity<Guid>
{
    public string? Url { get; set; }

    public string? Type { get; set; }

    public string? LicenseType { get; set; }

    public string? AttributionText { get; set; }

    public DateTime? CreatedAt { get; set; }

    public ICollection<ExerciseMedia> ExerciseMedia { get; set; } = new List<ExerciseMedia>();
}
