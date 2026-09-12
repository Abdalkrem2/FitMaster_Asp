using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FitMaster.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace FitMaster.Infrastructure.Files;

public class CloudinaryFileStorageService(IOptions<CloudinarySettings> settings) : IFileStorageService
{
    private readonly Lazy<Cloudinary> _cloudinary = new(() =>
    {
        var s = settings.Value;
        if (string.IsNullOrWhiteSpace(s.CloudName) || string.IsNullOrWhiteSpace(s.ApiKey) || string.IsNullOrWhiteSpace(s.ApiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary credentials are not fully set. Configure them via User Secrets: " +
                "dotnet user-secrets set \"Cloudinary:CloudName\" \"...\", " +
                "dotnet user-secrets set \"Cloudinary:ApiKey\" \"...\", " +
                "dotnet user-secrets set \"Cloudinary:ApiSecret\" \"...\"");
        }

        return new Cloudinary(new Account(s.CloudName, s.ApiKey, s.ApiSecret));
    });

    public async Task<UploadedFile> UploadImageAsync(Stream content, string fileName, string folder, CancellationToken cancellationToken)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            Folder = folder,
        };

        var result = await _cloudinary.Value.UploadAsync(uploadParams, cancellationToken);

        if (result.Error is not null)
        {
            throw new HttpRequestException($"Cloudinary upload failed: {result.Error.Message}");
        }

        return new UploadedFile(result.SecureUrl.ToString(), result.PublicId);
    }
}
