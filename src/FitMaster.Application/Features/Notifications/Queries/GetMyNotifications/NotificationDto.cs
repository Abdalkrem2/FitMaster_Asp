using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Notifications.Queries.GetMyNotifications;

public record NotificationDto(long NotificationId, NotificationType Type, string Message, string? Details, bool Read, DateTime CreatedAt);
