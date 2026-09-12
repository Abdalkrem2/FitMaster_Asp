using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Enums;
using MediatR;

namespace FitMaster.Application.Features.Packages.Commands.CreatePackage;

public class CreatePackageHandler(IApplicationDbContext db, ICurrentUserService currentUser, IPublisher publisher)
    : IRequestHandler<CreatePackageCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
    {
        var package = new Package
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            DurationInDays = request.DurationInDays,
        };

        db.Packages.Add(package);
        await db.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new ActivityOccurredEvent(
            currentUser.UserId!.Value, ActionType.Create, EntityType.Package, package.Id,
            $"Created package \"{package.Name}\" (${package.Price:0.00}, {package.DurationInDays} days)"), cancellationToken);

        return Result<long>.Success(package.Id);
    }
}
