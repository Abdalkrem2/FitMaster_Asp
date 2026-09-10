using FitMaster.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using FitMaster.Application.Features.Notifications.Queries.GetMyNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await sender.Send(new GetMyNotificationsQuery(), ct));

    [HttpPost("{notificationId:long}/read")]
    public async Task<IActionResult> MarkAsRead(long notificationId, CancellationToken ct)
    {
        var result = await sender.Send(new MarkNotificationAsReadCommand(notificationId), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
