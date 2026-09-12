using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Notifications.Queries.GetMyNotifications;

public record NotificationDto(long NotificationId, NotificationType Type, string Message, string? Details, bool Read, DateTime CreatedAt);

public record NotificationsPageDto(
    List<NotificationDto> Content,
    int PageNumber,
    int PageSize,
    int TotalElements,
    int TotalPages,
    bool LastPage,
    int UnreadCount);
