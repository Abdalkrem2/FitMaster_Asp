using FitMaster.Application.Features.Packages.Commands.CreatePackage;
using FitMaster.Application.Features.Packages.Commands.DeletePackage;
using FitMaster.Application.Features.Packages.Commands.UpdatePackage;
using FitMaster.Application.Features.Packages.Queries.GetPackageById;
using FitMaster.Application.Features.Packages.Queries.GetPackagesList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitMaster.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PackagesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] bool includeInactive, CancellationToken ct)
        => Ok(await sender.Send(new GetPackagesListQuery(includeInactive), ct));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
        => Ok(await sender.Send(new GetPackageByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreatePackageCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Errors);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdatePackageCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        var result = await sender.Send(command, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var result = await sender.Send(new DeletePackageCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
