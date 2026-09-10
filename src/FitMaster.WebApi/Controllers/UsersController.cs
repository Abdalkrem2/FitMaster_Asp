using FitMaster.Application.Features.Users.Commands.CreateStaffUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateStaffUser(CreateStaffUserCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }
}
