using FitMaster.Application.Pdf;
using Microsoft.Extensions.Logging;

namespace FitMaster.Infrastructure.Files;

/// <summary>
/// HttpClient is a typed client registered via AddHttpClient (see
/// DependencyInjection.cs), same pattern as GroqMealPlanClient - a short
/// timeout there, not a bare `new HttpClient()`.
/// </summary>
public class ExerciseImageFetcher(HttpClient httpClient, ILogger<ExerciseImageFetcher> logger) : IExerciseImageFetcher
{
    public async Task<byte[]?> FetchAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            return await httpClient.GetByteArrayAsync(url, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(ex, "Failed to fetch exercise image {Url} for PDF export", url);
            return null;
        }
    }
}
