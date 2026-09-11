using FitMaster.Application.Features.Exercises.Commands.UploadExerciseMedia;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

// The exercise catalog itself is read-only reference data (see Phase 2/6 notes) with
// no general browsing endpoint yet - this controller exists solely for the Phase 9
// media-upload action.
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class ExercisesController(ISender sender) : ControllerBase
{
    [HttpPost("{exerciseId:guid}/media")]
    public async Task<IActionResult> UploadMedia(
        Guid exerciseId, IFormFile file, [FromForm] string? role, [FromForm] string? sex, [FromForm] string? viewAngle, CancellationToken ct)
    {
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, ct);

        var result = await sender.Send(
            new UploadExerciseMediaCommand(exerciseId, stream.ToArray(), file.FileName, file.ContentType, role, sex, viewAngle), ct);
        return result.Succeeded ? Ok(new { mediaAssetId = result.Value }) : BadRequest(result.Errors);
    }
}
