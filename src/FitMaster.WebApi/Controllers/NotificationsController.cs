using FitMaster.Application.Features.Notifications.Commands.ClearAllNotifications;
using FitMaster.Application.Features.Notifications.Commands.DeleteNotification;
using FitMaster.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
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
    public async Task<IActionResult> GetMine([FromQuery] int page, [FromQuery] int size, CancellationToken ct)
        => Ok(await sender.Send(new GetMyNotificationsQuery(page, size), ct));

    [HttpPut("{notificationId:long}/read")]
    public async Task<IActionResult> MarkAsRead(long notificationId, CancellationToken ct)
    {
        var result = await sender.Send(new MarkNotificationAsReadCommand(notificationId), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var result = await sender.Send(new MarkAllNotificationsAsReadCommand(), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("{notificationId:long}")]
    public async Task<IActionResult> Delete(long notificationId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteNotificationCommand(notificationId), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearAll(CancellationToken ct)
    {
        var result = await sender.Send(new ClearAllNotificationsCommand(), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
