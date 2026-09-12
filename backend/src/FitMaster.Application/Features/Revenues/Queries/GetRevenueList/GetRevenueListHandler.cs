using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Revenues.Queries.GetRevenueList;

public class GetRevenueListHandler(IApplicationDbContext db) : IRequestHandler<GetRevenueListQuery, List<RevenueListItemDto>>
{
    public Task<List<RevenueListItemDto>> Handle(GetRevenueListQuery request, CancellationToken cancellationToken)
        => db.Revenues
            .Where(r => !request.From.HasValue || r.CreatedAt >= request.From)
            .Where(r => !request.To.HasValue || r.CreatedAt <= request.To)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RevenueListItemDto(
                r.Id,
                r.MemberId,
                r.Member.FullName,
                r.MembershipId,
                r.Membership.Package.Name,
                r.Amount,
                r.Description,
                r.CreatedAt))
            .ToListAsync(cancellationToken);
}
