using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Revenues.Queries.GetRevenueStats;

public class GetRevenueStatsHandler(IApplicationDbContext db) : IRequestHandler<GetRevenueStatsQuery, RevenueStatsDto>
{
    public async Task<RevenueStatsDto> Handle(GetRevenueStatsQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var yearStart = new DateOnly(today.Year, 1, 1);

        var todayTotal = await db.Revenues.Where(r => r.CreatedAt == today).SumAsync(r => (decimal?)r.Amount, cancellationToken) ?? 0;
        var monthTotal = await db.Revenues.Where(r => r.CreatedAt >= monthStart).SumAsync(r => (decimal?)r.Amount, cancellationToken) ?? 0;
        var yearTotal = await db.Revenues.Where(r => r.CreatedAt >= yearStart).SumAsync(r => (decimal?)r.Amount, cancellationToken) ?? 0;
        var debt = await db.Memberships
            .Where(m => m.Status == MembershipStatus.Active && m.Debt != null)
            .SumAsync(m => m.Debt!.Value, cancellationToken);

        return new RevenueStatsDto(todayTotal, monthTotal, yearTotal, debt);
    }
}
