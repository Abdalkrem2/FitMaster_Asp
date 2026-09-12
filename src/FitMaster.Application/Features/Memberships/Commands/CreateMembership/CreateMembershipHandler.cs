using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Application.MembershipProvisioning;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Memberships.Commands.CreateMembership;

public class CreateMembershipHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IPublisher publisher,
    IMembershipProvisioningService provisioning) : IRequestHandler<CreateMembershipCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateMembershipCommand request, CancellationToken cancellationToken)
    {
        var memberName = await db.Users
            .Where(u => u.Id == request.MemberId && !u.Deleted)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync(cancellationToken);
        if (memberName is null)
        {
            return Result<long>.Failure("Member not found.");
        }

        var package = await db.Packages
            .FirstOrDefaultAsync(p => p.Id == request.PackageId && !p.Deleted, cancellationToken);
        if (package is null)
        {
            return Result<long>.Failure("Package not found.");
        }
        if (package.Status != PackageStatus.Active)
        {
            return Result<long>.Failure("This package is not currently active and cannot be sold.");
        }

        var price = request.Price ?? package.Price;
        // Debt is always derived from price - amountPaid, never entered directly -
        // same rule as EndDate (see MembershipProvisioningService): keep it consistent
        // with what was actually sold and paid, rather than trusting a client-computed number.
        var amountPaid = request.AmountPaid ?? price;

        var membership = await provisioning.ProvisionAsync(
            request.MemberId, package, request.StartDate, price, amountPaid, request.Description,
            revenueCreatedById: currentUser.UserId!.Value,
            revenueDescription: $"Membership sale - {package.Name}",
            cancellationToken);

        await publisher.Publish(new ActivityOccurredEvent(
            currentUser.UserId!.Value, ActionType.Create, EntityType.Membership, membership.Id,
            $"Added \"{package.Name}\" subscription for \"{memberName}\" " +
            $"({membership.StartDate:yyyy-MM-dd} → {membership.EndDate:yyyy-MM-dd}, ${price:0.00})"), cancellationToken);

        return Result<long>.Success(membership.Id);
    }
}
