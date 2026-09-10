using FitMaster.Application.Features.Workouts.Commands.ArchiveWorkoutPlan;
using FitMaster.Application.Features.Workouts.Commands.GenerateWorkoutPlan;
using FitMaster.Application.Features.Workouts.Commands.RenameWorkoutPlan;
using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;
using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlansByMember;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class WorkoutPlansController(ISender sender) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
        => Ok(await sender.Send(new GetWorkoutPlanByIdQuery(id), ct));

    [HttpGet("by-member/{memberId:long}")]
    public async Task<IActionResult> GetByMember(long memberId, CancellationToken ct)
        => Ok(await sender.Send(new GetWorkoutPlansByMemberQuery(memberId), ct));

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(GenerateWorkoutPlanCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpPut("{id:long}/name")]
    public async Task<IActionResult> Rename(long id, [FromBody] string name, CancellationToken ct)
    {
        var result = await sender.Send(new RenameWorkoutPlanCommand(id, name), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("{id:long}/archive")]
    public async Task<IActionResult> Archive(long id, CancellationToken ct)
    {
        var result = await sender.Send(new ArchiveWorkoutPlanCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
