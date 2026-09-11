using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Workouts.Commands.ArchiveWorkoutPlan;
using FitMaster.Application.Features.Workouts.Commands.GenerateWorkoutPlan;
using FitMaster.Application.Features.Workouts.Commands.RenameWorkoutPlan;
using FitMaster.Application.Features.Workouts.Queries.GetActiveWorkoutPlan;
using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;
using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlansByMember;
using FitMaster.Application.Pdf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class WorkoutPlansController(ISender sender, ICurrentUserService currentUser, IWorkoutPlanPdfGenerator pdfGenerator) : ControllerBase
{
    // --- Self-service ("me") - any authenticated user, scoped via the JWT only ---

    [HttpPost("me/generate")]
    public async Task<IActionResult> GenerateMine(CancellationToken ct)
    {
        var result = await sender.Send(new GenerateWorkoutPlanCommand(currentUser.UserId!.Value), ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await sender.Send(new GetActiveWorkoutPlanQuery(currentUser.UserId!.Value), ct));

    [HttpGet("me/history")]
    public async Task<IActionResult> GetMyHistory(CancellationToken ct)
        => Ok(await sender.Send(new GetWorkoutPlansByMemberQuery(currentUser.UserId!.Value), ct));

    [HttpGet("me/active/pdf")]
    public async Task<IActionResult> GetMyActivePlanPdf(CancellationToken ct)
    {
        var plan = await sender.Send(new GetActiveWorkoutPlanQuery(currentUser.UserId!.Value), ct);
        var pdfBytes = pdfGenerator.Generate(plan);
        return File(pdfBytes, "application/pdf", "workout-plan.pdf");
    }

    // --- Admin/Employee acting on a member by id ---

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
        => Ok(await sender.Send(new GetWorkoutPlanByIdQuery(id), ct));

    [HttpGet("by-member/{memberId:long}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetByMember(long memberId, CancellationToken ct)
        => Ok(await sender.Send(new GetWorkoutPlansByMemberQuery(memberId), ct));

    [HttpPost("generate")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Generate(GenerateWorkoutPlanCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpPut("{id:long}/name")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Rename(long id, [FromBody] string name, CancellationToken ct)
    {
        var result = await sender.Send(new RenameWorkoutPlanCommand(id, name), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("{id:long}/archive")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Archive(long id, CancellationToken ct)
    {
        var result = await sender.Send(new ArchiveWorkoutPlanCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
