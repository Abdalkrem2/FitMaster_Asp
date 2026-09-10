using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Memberships.Commands.RenewMembership;

public class RenewMembershipHandler(IApplicationDbContext db) : IRequestHandler<RenewMembershipCommand, Result>
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

        var newStart = request.StartDate ?? membership.EndDate;
        membership.StartDate = newStart;
        membership.EndDate = newStart.AddDays(membership.Package.DurationInDays);
        membership.Status = MembershipStatus.Active;
        membership.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
