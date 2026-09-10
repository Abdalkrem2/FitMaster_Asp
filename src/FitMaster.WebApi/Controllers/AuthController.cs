using FitMaster.Application.Features.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

// Note: public self-registration (RegisterCommand/RegisterHandler in Application)
// is intentionally NOT exposed here. Members are onboarded by staff via
// POST /api/members (see MembersController, Admin/Employee only). Staff
// accounts (Employee/Admin) are created via POST /api/users (Admin only, see
// UsersController). RegisterCommand is kept in Application, unused for now -
// it's the starting point for a future "self-signup + online payment" flow.
[ApiController]
[Route("api/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Unauthorized(result.Errors);
    }
}
