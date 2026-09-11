namespace FitMaster.Application.Common;

public static class ImageUploadRules
{
    public const int MaxSizeBytes = 5 * 1024 * 1024;

    public static readonly IReadOnlyCollection<string> AllowedContentTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];
}
