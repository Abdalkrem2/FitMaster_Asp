using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Packages.Commands.DeletePackage;

public class DeletePackageHandler(IApplicationDbContext db, ICurrentUserService currentUser, IPublisher publisher)
    : IRequestHandler<DeletePackageCommand, Result>
{
    public async Task<Result> Handle(DeletePackageCommand request, CancellationToken cancellationToken)
    {
        var package = await db.Packages
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.Deleted, cancellationToken);

        if (package is null)
        {
            return Result.Failure("Package not found.");
        }

        package.Deleted = true;
        package.Status = PackageStatus.Inactive;

        await db.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new ActivityOccurredEvent(
            currentUser.UserId!.Value, ActionType.Delete, EntityType.Package, package.Id,
            $"Deleted package \"{package.Name}\""), cancellationToken);

        return Result.Success();
    }
}
