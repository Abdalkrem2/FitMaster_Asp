using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Payments;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Memberships;


public class Package : BaseEntity
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public required decimal Price { get; set; }

    public required int DurationInDays { get; set; }

    public PackageStatus Status { get; set; } = PackageStatus.Active;

    public bool Deleted { get; set; }

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
