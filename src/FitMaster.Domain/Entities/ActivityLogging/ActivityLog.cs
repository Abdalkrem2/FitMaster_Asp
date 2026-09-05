using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.ActivityLogging;


public class ActivityLog : BaseEntity
{
    public required long PerformedById { get; set; }

    public User PerformedBy { get; set; } = null!;

    public required ActionType Action { get; set; }

    public EntityType? EntityType { get; set; }

    public required long EntityId { get; set; }

    public string? Details { get; set; }

    public DateTime CreatedAt { get; set; }
}
