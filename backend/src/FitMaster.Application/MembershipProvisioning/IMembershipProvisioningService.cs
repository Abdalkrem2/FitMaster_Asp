using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.MembershipProvisioning;

/// <summary>
/// Creates a Membership + its Revenue row, applying the one stacking rule every
/// membership purchase needs regardless of who/what initiated it (a staff sale via
/// CreateMembershipHandler, or a confirmed online payment via the Stripe webhook):
/// if the member already has active, unexpired coverage, the new membership starts
/// right after it ends instead of overlapping it. Extracted out of
/// CreateMembershipHandler so both callers share the exact same rule instead of
/// re-implementing it.
/// </summary>
public interface IMembershipProvisioningService
{
    Task<Membership> ProvisionAsync(
        long memberId,
        Package package,
        DateOnly requestedStartDate,
        decimal price,
        decimal amountPaid,
        string? description,
        long revenueCreatedById,
        string revenueDescription,
        CancellationToken cancellationToken);
}

public class MembershipProvisioningService(IApplicationDbContext db) : IMembershipProvisioningService
{
    public async Task<Membership> ProvisionAsync(
        long memberId,
        Package package,
        DateOnly requestedStartDate,
        decimal price,
        decimal amountPaid,
        string? description,
        long revenueCreatedById,
        string revenueDescription,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // If the member already has active, unexpired coverage, queue the new
        // membership to start right after it ends instead of today - otherwise
        // a new purchase would overlap the existing one instead of stacking.
        // Picks the furthest-out end date if more than one active row qualifies.
        var currentCoverageEndDate = await db.Memberships
            .Where(m => m.MemberId == memberId
                && m.Status == MembershipStatus.Active
                && m.EndDate > requestedStartDate)
            .OrderByDescending(m => m.EndDate)
            .Select(m => (DateOnly?)m.EndDate)
            .FirstOrDefaultAsync(cancellationToken);

        var effectiveStartDate = currentCoverageEndDate ?? requestedStartDate;

        var membership = new Membership
        {
            MemberId = memberId,
            PackageId = package.Id,
            Status = MembershipStatus.Active,
            StartDate = effectiveStartDate,
            // The core rule: end date is always derived from the package's duration,
            // never entered manually - keeps it consistent with what was actually sold.
            EndDate = effectiveStartDate.AddDays(package.DurationInDays),
            Price = price,
            Debt = Math.Max(price - amountPaid, 0),
            Description = description,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Memberships.Add(membership);
        await db.SaveChangesAsync(cancellationToken);

        db.Revenues.Add(new Revenue
        {
            MemberId = memberId,
            MembershipId = membership.Id,
            CreatedById = revenueCreatedById,
            Amount = membership.Price,
            Description = revenueDescription,
            CreatedAt = DateOnly.FromDateTime(now),
            UpdatedAt = DateOnly.FromDateTime(now),
        });
        await db.SaveChangesAsync(cancellationToken);

        return membership;
    }
}
