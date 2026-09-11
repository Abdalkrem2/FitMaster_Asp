namespace FitMaster.Application.Common.Interfaces;

public record UploadedFile(string Url, string PublicId);

/// <summary>
/// Uploads a file to external storage. Implemented in Infrastructure via
/// Cloudinary - Handlers depend on this instead of the Cloudinary SDK directly.
/// </summary>
public interface IFileStorageService
{
    Task<UploadedFile> UploadImageAsync(Stream content, string fileName, string folder, CancellationToken cancellationToken);
}
