using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Packages.Commands.UpdatePackage;

public class UpdatePackageHandler(IApplicationDbContext db, ICurrentUserService currentUser, IPublisher publisher)
    : IRequestHandler<UpdatePackageCommand, Result>
{
    public async Task<Result> Handle(UpdatePackageCommand request, CancellationToken cancellationToken)
    {
        var package = await db.Packages
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.Deleted, cancellationToken);

        if (package is null)
        {
            return Result.Failure("Package not found.");
        }

        var oldName = package.Name;
        var changes = new List<string>();
        if (package.Name != request.Name) changes.Add($"name \"{package.Name}\" → \"{request.Name}\"");
        if (package.Price != request.Price) changes.Add($"price ${package.Price:0.00} → ${request.Price:0.00}");
        if (package.DurationInDays != request.DurationInDays) changes.Add($"duration {package.DurationInDays} → {request.DurationInDays} days");
        if (package.Status != request.Status) changes.Add($"status {package.Status} → {request.Status}");

        package.Name = request.Name;
        package.Description = request.Description;
        package.Price = request.Price;
        package.DurationInDays = request.DurationInDays;
        package.Status = request.Status;

        await db.SaveChangesAsync(cancellationToken);

        var details = changes.Count > 0
            ? $"Updated package \"{oldName}\" ({string.Join(", ", changes)})"
            : $"Updated package \"{oldName}\" (no field changes)";
        await publisher.Publish(new ActivityOccurredEvent(
            currentUser.UserId!.Value, ActionType.Update, EntityType.Package, package.Id, details), cancellationToken);

        return Result.Success();
    }
}
