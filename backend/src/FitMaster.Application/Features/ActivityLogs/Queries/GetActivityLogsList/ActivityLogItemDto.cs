using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.ActivityLogs.Queries.GetActivityLogsList;

public record ActivityLogItemDto(
    long Id,
    ActionType ActionType,
    EntityType? EntityType,
    long EntityId,
    string PerformedByName,
    DateTime CreatedAt,
    string? Details);
