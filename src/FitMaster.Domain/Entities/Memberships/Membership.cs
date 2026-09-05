using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Memberships;

public class Membership : BaseEntity
{
    public required long MemberId { get; set; }

    public User Member { get; set; } = null!;

    public required long PackageId { get; set; }

    public Package Package { get; set; } = null!;

    public MembershipStatus Status { get; set; } = MembershipStatus.Active;

    public required DateOnly StartDate { get; set; }

    public required DateOnly EndDate { get; set; }

    public decimal Price { get; set; }

    /// <summary>Outstanding amount the member still owes, if any.</summary>
    public decimal? Debt { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();
}
