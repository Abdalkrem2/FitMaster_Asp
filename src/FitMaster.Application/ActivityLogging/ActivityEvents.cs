using MediatR;

namespace FitMaster.Application.ActivityLogging;

/// <summary>
/// Domain events published by handlers right after their own SaveChangesAsync
/// succeeds. A single ActivityLogEventHandler subscribes to all of them, writing
/// the ActivityLog row and fanning out a Notification - keeps each handler
/// focused on its own job instead of mixing in logging/notification boilerplate.
/// </summary>
public record MemberCreatedEvent(long MemberId, long PerformedById) : INotification;

public record StaffUserCreatedEvent(long UserId, long PerformedById) : INotification;

public record MembershipCreatedEvent(long MembershipId, long PerformedById) : INotification;

public record MembershipRenewedEvent(long MembershipId, long PerformedById) : INotification;
