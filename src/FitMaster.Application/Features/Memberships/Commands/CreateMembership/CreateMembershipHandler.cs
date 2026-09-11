using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Memberships.Commands.CreateMembership;

public class CreateMembershipHandler(IApplicationDbContext db, ICurrentUserService currentUser, IPublisher publisher)
    : IRequestHandler<CreateMembershipCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateMembershipCommand request, CancellationToken cancellationToken)
    {
        var memberExists = await db.Users.AnyAsync(u => u.Id == request.MemberId && !u.Deleted, cancellationToken);
        if (!memberExists)
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

        var now = DateTime.UtcNow;

        var membership = new Membership
        {
            MemberId = request.MemberId,
            PackageId = request.PackageId,
            Status = MembershipStatus.Active,
            StartDate = request.StartDate,
            // The core rule: end date is always derived from the package's duration,
            // never entered manually - keeps it consistent with what was actually sold.
            EndDate = request.StartDate.AddDays(package.DurationInDays),
            Price = request.Price ?? package.Price,
            Debt = request.Debt,
            Description = request.Description,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Memberships.Add(membership);
        await db.SaveChangesAsync(cancellationToken);

        db.Revenues.Add(new Revenue
        {
            MemberId = request.MemberId,
            MembershipId = membership.Id,
            CreatedById = currentUser.UserId!.Value,
            Amount = membership.Price,
            Description = $"Membership sale - {package.Name}",
            CreatedAt = DateOnly.FromDateTime(now),
            UpdatedAt = DateOnly.FromDateTime(now),
        });
        await db.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new MembershipCreatedEvent(membership.Id, currentUser.UserId!.Value), cancellationToken);

        return Result<long>.Success(membership.Id);
    }
}
