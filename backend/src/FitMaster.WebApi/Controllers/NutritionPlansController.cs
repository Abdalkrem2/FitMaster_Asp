using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Nutrition.Commands.ArchiveNutritionPlan;
using FitMaster.Application.Features.Nutrition.Commands.GenerateNutritionPlan;
using FitMaster.Application.Features.Nutrition.Queries.GetActiveNutritionPlan;
using FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlanById;
using FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlansByMember;
using FitMaster.Application.Pdf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NutritionPlansController(ISender sender, ICurrentUserService currentUser, INutritionPlanPdfGenerator pdfGenerator) : ControllerBase
{
    // --- Self-service ("me") - any authenticated user, scoped via the JWT only ---

    [HttpPost("me/generate")]
    public async Task<IActionResult> GenerateMine(CancellationToken ct)
    {
        var result = await sender.Send(new GenerateNutritionPlanCommand(currentUser.UserId!.Value), ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await sender.Send(new GetActiveNutritionPlanQuery(currentUser.UserId!.Value), ct));

    [HttpGet("me/history")]
    public async Task<IActionResult> GetMyHistory(CancellationToken ct)
        => Ok(await sender.Send(new GetNutritionPlansByMemberQuery(currentUser.UserId!.Value), ct));

    [HttpGet("me/active/pdf")]
    public async Task<IActionResult> GetMyActivePlanPdf(CancellationToken ct)
    {
        var plan = await sender.Send(new GetActiveNutritionPlanQuery(currentUser.UserId!.Value), ct);
        var pdfBytes = pdfGenerator.Generate(plan);
        return File(pdfBytes, "application/pdf", "nutrition-plan.pdf");
    }

    // --- Admin/Employee acting on a member by id ---

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
        => Ok(await sender.Send(new GetNutritionPlanByIdQuery(id), ct));

    [HttpGet("by-member/{memberId:long}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetByMember(long memberId, CancellationToken ct)
        => Ok(await sender.Send(new GetNutritionPlansByMemberQuery(memberId), ct));

    [HttpPost("generate")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Generate(GenerateNutritionPlanCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpPost("{id:long}/archive")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Archive(long id, CancellationToken ct)
    {
        var result = await sender.Send(new ArchiveNutritionPlanCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
