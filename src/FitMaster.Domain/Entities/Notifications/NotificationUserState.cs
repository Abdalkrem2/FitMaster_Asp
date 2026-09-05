using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Identity;

namespace FitMaster.Domain.Entities.Notifications;

/// <summary>Tracks whether a specific user has read/dismissed a given Notification.</summary>
public class NotificationUserState : BaseEntity
{
    public required long NotificationId { get; set; }

    public Notification Notification { get; set; } = null!;

    public required long UserId { get; set; }

    public User User { get; set; } = null!;

    public bool Read { get; set; }

    public bool Deleted { get; set; }

    /// <summary>When this notification was assigned/surfaced to the user.</summary>
    public DateTime AssignedAt { get; set; }
}
