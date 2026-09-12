using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.ActivityLogs.Queries.GetActivityLogsList;

public class GetActivityLogsListHandler(IApplicationDbContext db)
    : IRequestHandler<GetActivityLogsListQuery, PagedResult<ActivityLogItemDto>>
{
    public async Task<PagedResult<ActivityLogItemDto>> Handle(GetActivityLogsListQuery request, CancellationToken cancellationToken)
    {
        var query = db.ActivityLogs.AsQueryable();

        if (request.PerformedBy is { } performedBy)
        {
            query = query.Where(a => a.PerformedById == performedBy);
        }
        if (request.EntityType is { } entityType)
        {
            query = query.Where(a => a.EntityType == entityType);
        }

        var totalElements = await query.CountAsync(cancellationToken);

        var page = Math.Max(request.Page, 0);
        var size = request.Size <= 0 ? 20 : request.Size;

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip(page * size)
            .Take(size)
            .Select(a => new ActivityLogItemDto(
                a.Id,
                a.Action,
                a.EntityType,
                a.EntityId,
                a.PerformedBy.FullName,
                a.CreatedAt,
                a.Details))
            .ToListAsync(cancellationToken);

        return PagedResult<ActivityLogItemDto>.Create(items, totalElements, page, size);
    }
}
