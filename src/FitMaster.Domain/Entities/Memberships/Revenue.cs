using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Identity;

namespace FitMaster.Domain.Entities.Memberships;

/// <summary>A single recorded income transaction (e.g. a membership payment).</summary>
public class Revenue : BaseEntity
{
    public required long MemberId { get; set; }

    public User Member { get; set; } = null!;

    public required long MembershipId { get; set; }

    public Membership Membership { get; set; } = null!;

    /// <summary>The staff member (employee/admin) who recorded this transaction.</summary>
    public required long CreatedById { get; set; }

    public User CreatedBy { get; set; } = null!;

    public required decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }
}
