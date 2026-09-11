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
        EntityType? parsedEntityType = Enum.TryParse<EntityType>(entityType, ignoreCase: true, out var parsed) ? parsed : null;
        return Ok(await sender.Send(new GetActivityLogsListQuery(performedBy, parsedEntityType, page, size), ct));
    }
}
