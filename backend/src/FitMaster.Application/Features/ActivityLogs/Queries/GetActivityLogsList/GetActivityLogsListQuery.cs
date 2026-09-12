using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;

namespace FitMaster.Application.Features.ActivityLogs.Queries.GetActivityLogsList;

public record GetActivityLogsListQuery(
    long? PerformedBy,
    EntityType? EntityType,
    int Page = 0,
    int Size = 20) : IRequest<PagedResult<ActivityLogItemDto>>;
