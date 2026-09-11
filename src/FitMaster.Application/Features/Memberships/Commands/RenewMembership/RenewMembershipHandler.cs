using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Memberships.Commands.RenewMembership;

public class RenewMembershipHandler(IApplicationDbContext db, ICurrentUserService currentUser, IPublisher publisher)
    : IRequestHandler<RenewMembershipCommand, Result>
{
    public async Task<Result> Handle(RenewMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await db.Memberships
            .Include(m => m.Package)
            .FirstOrDefaultAsync(m => m.Id == request.MembershipId, cancellationToken);

        if (membership is null)
        {
            return Result.Failure("Membership not found.");
        }

        var now = DateTime.UtcNow;
        var newStart = request.StartDate ?? membership.EndDate;
        membership.StartDate = newStart;
        membership.EndDate = newStart.AddDays(membership.Package.DurationInDays);
        membership.Status = MembershipStatus.Active;
        membership.UpdatedAt = now;

        await db.SaveChangesAsync(cancellationToken);

        db.Revenues.Add(new Revenue
        {
            MemberId = membership.MemberId,
            MembershipId = membership.Id,
            CreatedById = currentUser.UserId!.Value,
            Amount = membership.Price,
            Description = $"Membership renewal - {membership.Package.Name}",
            CreatedAt = DateOnly.FromDateTime(now),
            UpdatedAt = DateOnly.FromDateTime(now),
        });
        await db.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new MembershipRenewedEvent(membership.Id, currentUser.UserId!.Value), cancellationToken);

        return Result.Success();
    }
}
