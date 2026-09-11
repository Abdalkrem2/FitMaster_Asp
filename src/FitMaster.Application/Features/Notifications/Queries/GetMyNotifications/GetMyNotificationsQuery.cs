using MediatR;

namespace FitMaster.Application.Features.Notifications.Queries.GetMyNotifications;

/// <summary>Lists the current user's notifications, newest first.</summary>
public record GetMyNotificationsQuery : IRequest<List<NotificationDto>>;
