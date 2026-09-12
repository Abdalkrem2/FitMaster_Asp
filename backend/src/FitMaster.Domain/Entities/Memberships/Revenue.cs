using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Identity;

namespace FitMaster.Domain.Entities.Memberships;


public class Revenue : BaseEntity
{
    public required long MemberId { get; set; }

    public User Member { get; set; } = null!;

    public required long MembershipId { get; set; }

    public Membership Membership { get; set; } = null!;

    public required long CreatedById { get; set; }

    public User CreatedBy { get; set; } = null!;

    public required decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }
}
