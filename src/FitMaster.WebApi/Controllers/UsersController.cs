using FitMaster.Application.Features.Users.Commands.CreateStaffUser;
using FitMaster.Application.Features.Users.Commands.DeleteStaffUser;
using FitMaster.Application.Features.Users.Commands.UpdateStaffUser;
using FitMaster.Application.Features.Users.Queries.GetStaffUsersList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page, [FromQuery] int size, CancellationToken ct)
        => Ok(await sender.Send(new GetStaffUsersListQuery(page, size), ct));

    [HttpPost]
    public async Task<IActionResult> CreateStaffUser(CreateStaffUserCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPatch("{userId:long}")]
    public async Task<IActionResult> Update(long userId, UpdateStaffUserCommand command, CancellationToken ct)
    {
        if (userId != command.UserId) return BadRequest("Route id and body id must match.");
        var result = await sender.Send(command, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("{userId:long}")]
    public async Task<IActionResult> Delete(long userId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteStaffUserCommand(userId), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
