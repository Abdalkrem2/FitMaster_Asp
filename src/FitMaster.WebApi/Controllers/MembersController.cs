using FitMaster.Application.Features.Members.Commands.CreateMember;
using FitMaster.Application.Features.Members.Commands.UpdateMemberProfile;
using FitMaster.Application.Features.Members.Commands.UploadProfilePicture;
using FitMaster.Application.Features.Members.Queries.GetMemberById;
using FitMaster.Application.Features.Members.Queries.GetMembersList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Route("api/[controller]")]
public class MembersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? search, CancellationToken ct)
        => Ok(await sender.Send(new GetMembersListQuery(search), ct));

    [HttpGet("{memberId:long}")]
    public async Task<IActionResult> GetById(long memberId, CancellationToken ct)
        => Ok(await sender.Send(new GetMemberByIdQuery(memberId), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { memberId = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpPut("{memberId:long}")]
    public async Task<IActionResult> UpdateProfile(long memberId, UpdateMemberProfileCommand command, CancellationToken ct)
    {
        if (memberId != command.MemberId) return BadRequest("Route id and body id must match.");
        var result = await sender.Send(command, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("{memberId:long}/profile-picture")]
    public async Task<IActionResult> UploadProfilePicture(long memberId, IFormFile file, CancellationToken ct)
    {
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, ct);

        var result = await sender.Send(
            new UploadProfilePictureCommand(memberId, stream.ToArray(), file.FileName, file.ContentType), ct);
        return result.Succeeded ? Ok(new { url = result.Value }) : BadRequest(result.Errors);
    }
}
