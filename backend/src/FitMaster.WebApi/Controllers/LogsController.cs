using FitMaster.Application.Features.ActivityLogs.Queries.GetActivityLogsList;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class LogsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] long? performedBy, [FromQuery] string? entityType, [FromQuery] int page, [FromQuery] int size, CancellationToken ct)
    {
        // entityType arrives as SCREAMING_SNAKE_CASE (e.g. "WORKOUT_PLAN") to match the
        // frontend's JsonStringEnumConverter(SnakeCaseUpper) convention - Enum.TryParse only
        // understands the literal member name ("WorkoutPlan"), so strip underscores first.
        EntityType? parsedEntityType = entityType is not null
            && Enum.TryParse<EntityType>(entityType.Replace("_", ""), ignoreCase: true, out var parsed)
                ? parsed
                : null;
        return Ok(await sender.Send(new GetActivityLogsListQuery(performedBy, parsedEntityType, page, size), ct));
    }
}
