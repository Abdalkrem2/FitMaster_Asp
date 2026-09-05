using FitMaster.Domain.Common;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Identity;

/// <summary>A single application role (Admin / Member / Employee).</summary>
public class Role : BaseEntity
{
    public required AppRole RoleName { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
