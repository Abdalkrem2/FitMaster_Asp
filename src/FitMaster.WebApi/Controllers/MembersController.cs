using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Members.Commands.ChangePassword;
using FitMaster.Application.Features.Members.Commands.CreateMember;
using FitMaster.Application.Features.Members.Commands.UpdateMemberIdentity;
using FitMaster.Application.Features.Members.Commands.UpdateMemberProfile;
using FitMaster.Application.Features.Members.Commands.UploadProfilePicture;
using FitMaster.Application.Features.Members.Queries.GetMemberById;
using FitMaster.Application.Features.Members.Queries.GetMembersList;
using FitMaster.Application.Features.Members.Queries.GetMemberStats;
using FitMaster.Application.Features.Members.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MembersController(ISender sender, ICurrentUserService currentUser) : ControllerBase
{
    // --- Self-service ("me") - any authenticated user, scoped via the JWT only ---

    [HttpGet("me")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await sender.Send(new GetMemberByIdQuery(currentUser.UserId!.Value), ct));

    [HttpGet("me/profile")]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
        => Ok(await sender.Send(new GetMyProfileQuery(currentUser.UserId!.Value), ct));

    [HttpPatch("me/profile")]
    public async Task<IActionResult> UpdateMyProfile(UpdateMemberProfileCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { MemberId = currentUser.UserId!.Value }, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangeMyPassword(ChangePasswordCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { UserId = currentUser.UserId!.Value }, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    // --- Admin/Employee managing members by id ---

    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetList([FromQuery] string? search, [FromQuery] int page, [FromQuery] int size, CancellationToken ct)
        => Ok(await sender.Send(new GetMembersListQuery(search, page, size), ct));

    [HttpGet("stats")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
        => Ok(await sender.Send(new GetMemberStatsQuery(), ct));

    [HttpGet("{memberId:long}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetById(long memberId, CancellationToken ct)
        => Ok(await sender.Send(new GetMemberByIdQuery(memberId), ct));

    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Create(CreateMemberCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { memberId = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpPut("{memberId:long}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> UpdateProfile(long memberId, UpdateMemberProfileCommand command, CancellationToken ct)
    {
        if (memberId != command.MemberId) return BadRequest("Route id and body id must match.");
        var result = await sender.Send(command, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPatch("{memberId:long}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> UpdateIdentity(long memberId, UpdateMemberIdentityCommand command, CancellationToken ct)
    {
        if (memberId != command.MemberId) return BadRequest("Route id and body id must match.");
        var result = await sender.Send(command, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("{memberId:long}/profile-picture")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> UploadProfilePicture(long memberId, IFormFile file, CancellationToken ct)
    {
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, ct);

        var result = await sender.Send(
            new UploadProfilePictureCommand(memberId, stream.ToArray(), file.FileName, file.ContentType), ct);
        return result.Succeeded ? Ok(new { url = result.Value }) : BadRequest(result.Errors);
    }
}
