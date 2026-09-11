using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Queries.GetMembersList;

public class GetMembersListHandler(IApplicationDbContext db)
    : IRequestHandler<GetMembersListQuery, PagedResult<MemberListItemDto>>
{
    public async Task<PagedResult<MemberListItemDto>> Handle(GetMembersListQuery request, CancellationToken cancellationToken)
    {
        var query = db.MemberProfiles.Where(p => !p.Member.Deleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(p => p.Member.FullName.Contains(term) || p.Member.Phone.Contains(term));
        }

        var totalElements = await query.CountAsync(cancellationToken);

        var page = Math.Max(request.Page, 0);
        var size = request.Size <= 0 ? 20 : request.Size;

        var items = await query
            .OrderBy(p => p.Member.FullName)
            .Skip(page * size)
            .Take(size)
            .Select(p => new MemberListItemDto(
                p.MemberId,
                p.Member.Phone,
                p.Member.FullName,
                p.Member.Gender,
                p.Member.CreatedBy != null ? p.Member.CreatedBy.FullName : null,
                p.Member.Memberships
                    .Where(m => m.Status == MembershipStatus.Active)
                    .OrderByDescending(m => m.EndDate)
                    .Select(m => (decimal?)m.Debt)
                    .FirstOrDefault(),
                p.Member.Memberships
                    .Where(m => m.Status == MembershipStatus.Active)
                    .OrderByDescending(m => m.EndDate)
                    .Select(m => (DateOnly?)m.EndDate)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return PagedResult<MemberListItemDto>.Create(items, totalElements, page, size);
    }
}
