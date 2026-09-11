using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Revenues.Queries.GetMonthlyRevenue;

public class GetMonthlyRevenueHandler(IApplicationDbContext db) : IRequestHandler<GetMonthlyRevenueQuery, MonthlyRevenueDto>
{
    public async Task<MonthlyRevenueDto> Handle(GetMonthlyRevenueQuery request, CancellationToken cancellationToken)
    {
        var yearStart = new DateOnly(request.Year, 1, 1);
        var yearEnd = new DateOnly(request.Year, 12, 31);

        var byMonth = await db.Revenues
            .Where(r => r.CreatedAt >= yearStart && r.CreatedAt <= yearEnd)
            .GroupBy(r => r.CreatedAt.Month)
            .Select(g => new { Month = g.Key, Total = g.Sum(r => r.Amount) })
            .ToListAsync(cancellationToken);

        var months = Enumerable.Range(1, 12).ToDictionary(m => m, m => byMonth.FirstOrDefault(x => x.Month == m)?.Total ?? 0m);

        return new MonthlyRevenueDto(months.Values.Sum(), months);
    }
}
