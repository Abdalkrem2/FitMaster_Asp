using FitMaster.Application.Features.Revenues.Queries.GetRevenueList;
using FitMaster.Application.Features.Revenues.Queries.GetRevenueSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class RevenueController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct)
        => Ok(await sender.Send(new GetRevenueListQuery(from, to), ct));

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct)
        => Ok(await sender.Send(new GetRevenueSummaryQuery(from, to), ct));
}
