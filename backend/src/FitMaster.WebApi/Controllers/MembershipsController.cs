using FitMaster.Application.Features.Memberships.Commands.CreateMembership;
using FitMaster.Application.Features.Memberships.Commands.RenewMembership;
using FitMaster.Application.Features.Memberships.Queries.GetMembershipById;
using FitMaster.Application.Features.Memberships.Queries.GetMembershipsByMember;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MembershipsController(ISender sender) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
        => Ok(await sender.Send(new GetMembershipByIdQuery(id), ct));

    [HttpGet("by-member/{memberId:long}")]
    public async Task<IActionResult> GetByMember(long memberId, CancellationToken ct)
        => Ok(await sender.Send(new GetMembershipsByMemberQuery(memberId), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateMembershipCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpPost("{id:long}/renew")]
    public async Task<IActionResult> Renew(long id, [FromBody] DateOnly? startDate, CancellationToken ct)
    {
        var result = await sender.Send(new RenewMembershipCommand(id, startDate), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
