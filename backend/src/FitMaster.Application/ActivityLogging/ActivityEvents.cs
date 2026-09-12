using FitMaster.Domain.Enums;
using MediatR;

namespace FitMaster.Application.ActivityLogging;

/// <summary>
/// Single domain event covering every activity-loggable action. Published by a
/// handler right after its own SaveChangesAsync succeeds; a single
/// ActivityLogEventHandler subscribes to it, writing the ActivityLog row and
/// fanning out a Notification - keeps each handler focused on its own job
/// instead of mixing in logging/notification boilerplate.
///
/// The publishing handler builds Details itself, using data it already has in
/// scope (names, dates, amounts) - never something the reader resolves later by
/// joining on EntityId. That's the point of keeping EntityId and Details
/// separate: EntityId is for programmatic linking to the still-existing entity,
/// Details is the permanent human-readable record that must keep reading
/// correctly even if that entity is later renamed or deleted.
/// </summary>
public record ActivityOccurredEvent(
    long PerformedById,
    ActionType Action,
    EntityType EntityType,
    long EntityId,
    string Details) : INotification;
