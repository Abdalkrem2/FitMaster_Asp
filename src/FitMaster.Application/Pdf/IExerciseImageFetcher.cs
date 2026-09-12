namespace FitMaster.Application.Pdf;

/// <summary>
/// Downloads an exercise demonstration image (Cloudinary-hosted GIF/image) for
/// embedding in the workout plan PDF. Returns null instead of throwing on any
/// failure (timeout, 404, unreachable) - a missing image should never take down
/// the whole PDF, the exercise row just renders without one.
/// </summary>
public interface IExerciseImageFetcher
{
    Task<byte[]?> FetchAsync(string url, CancellationToken cancellationToken);
}
