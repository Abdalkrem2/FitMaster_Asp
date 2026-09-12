using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Revenues.Queries.GetRevenueSummary;

public class GetRevenueSummaryHandler(IApplicationDbContext db) : IRequestHandler<GetRevenueSummaryQuery, RevenueSummaryDto>
{
    public async Task<RevenueSummaryDto> Handle(GetRevenueSummaryQuery request, CancellationToken cancellationToken)
    {
        var grouped = await db.Revenues
            .Where(r => !request.From.HasValue || r.CreatedAt >= request.From)
            .Where(r => !request.To.HasValue || r.CreatedAt <= request.To)
            .GroupBy(r => r.Membership.Package.Name)
            .Select(g => new RevenueByPackageDto(g.Key, g.Count(), g.Sum(r => r.Amount)))
            .ToListAsync(cancellationToken);

        var byPackage = grouped.OrderByDescending(p => p.Amount).ToList();

        return new RevenueSummaryDto(byPackage.Sum(p => p.Amount), byPackage.Sum(p => p.Count), byPackage);
    }
}
