using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Exercises;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Exercises.Commands.UploadExerciseMedia;

public class UploadExerciseMediaHandler(IApplicationDbContext db, IFileStorageService fileStorage)
    : IRequestHandler<UploadExerciseMediaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UploadExerciseMediaCommand request, CancellationToken cancellationToken)
    {
        var exerciseExists = await db.Exercises.AnyAsync(e => e.Id == request.ExerciseId, cancellationToken);
        if (!exerciseExists)
        {
            return Result<Guid>.Failure("Exercise not found.");
        }

        await using var stream = new MemoryStream(request.Content);
        var uploaded = await fileStorage.UploadImageAsync(stream, request.FileName, "exercises", cancellationToken);

        var mediaAsset = new MediaAsset
        {
            Id = Guid.NewGuid(),
            Url = uploaded.Url,
            Type = "IMAGE",
            LicenseType = "OWN",
            CreatedAt = DateTime.UtcNow,
        };
        db.MediaAssets.Add(mediaAsset);

        db.ExerciseMedia.Add(new ExerciseMedia
        {
            ExerciseId = request.ExerciseId,
            MediaAssetId = mediaAsset.Id,
            Role = request.Role,
            Sex = request.Sex,
            ViewAngle = request.ViewAngle,
        });

        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(mediaAsset.Id);
    }
}
