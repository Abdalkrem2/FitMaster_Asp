using FitMaster.Domain.Common;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Notifications;

/// <summary>
/// A single notification event. Per-user read/dismiss state lives separately
/// in <see cref="NotificationUserState"/> so the same notification can be
/// broadcast to several users (e.g. all admins).
/// </summary>
public class Notification : BaseEntity
{
    public required NotificationType Type { get; set; }

    /// <summary>Id of the entity this notification is about (e.g. an ActivityLog id).</summary>
    public long? ReferenceId { get; set; }

    public required string Message { get; set; }

    public string? Details { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<NotificationUserState> UserStates { get; set; } = new List<NotificationUserState>();
}
