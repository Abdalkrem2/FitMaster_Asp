using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Revenues.Queries.GetRevenuePeriod;

public class GetRevenuePeriodHandler(IApplicationDbContext db) : IRequestHandler<GetRevenuePeriodQuery, RevenuePeriodDto>
{
    public async Task<RevenuePeriodDto> Handle(GetRevenuePeriodQuery request, CancellationToken cancellationToken)
    {
        var rows = await db.Revenues
            .Where(r => r.CreatedAt >= request.Start && r.CreatedAt <= request.End)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RevenueRowDto(
                r.Id,
                r.CreatedBy.FullName,
                r.Member.FullName,
                r.Amount,
                r.Membership.Package.Name,
                r.Membership.Debt,
                r.Description,
                r.CreatedAt))
            .ToListAsync(cancellationToken);

        return new RevenuePeriodDto(rows.Sum(r => r.Amount), rows.Sum(r => r.Debt ?? 0), rows);
    }
}
