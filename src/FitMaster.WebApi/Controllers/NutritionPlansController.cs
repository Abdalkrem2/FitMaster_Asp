using FitMaster.Application.Features.Nutrition.Commands.ArchiveNutritionPlan;
using FitMaster.Application.Features.Nutrition.Commands.GenerateNutritionPlan;
using FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlanById;
using FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlansByMember;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NutritionPlansController(ISender sender) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
        => Ok(await sender.Send(new GetNutritionPlanByIdQuery(id), ct));

    [HttpGet("by-member/{memberId:long}")]
    public async Task<IActionResult> GetByMember(long memberId, CancellationToken ct)
        => Ok(await sender.Send(new GetNutritionPlansByMemberQuery(memberId), ct));

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(GenerateNutritionPlanCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpPost("{id:long}/archive")]
    public async Task<IActionResult> Archive(long id, CancellationToken ct)
    {
        var result = await sender.Send(new ArchiveNutritionPlanCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
