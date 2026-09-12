using FitMaster.Application.Features.Revenues.Queries.GetMonthlyRevenue;
using FitMaster.Application.Features.Revenues.Queries.GetRevenueList;
using FitMaster.Application.Features.Revenues.Queries.GetRevenuePeriod;
using FitMaster.Application.Features.Revenues.Queries.GetRevenueStats;
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

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
        => Ok(await sender.Send(new GetRevenueStatsQuery(), ct));

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly([FromQuery] int year, CancellationToken ct)
        => Ok(await sender.Send(new GetMonthlyRevenueQuery(year), ct));

    [HttpGet("period")]
    public async Task<IActionResult> GetPeriod([FromQuery] DateOnly start, [FromQuery] DateOnly end, CancellationToken ct)
        => Ok(await sender.Send(new GetRevenuePeriodQuery(start, end), ct));
}
