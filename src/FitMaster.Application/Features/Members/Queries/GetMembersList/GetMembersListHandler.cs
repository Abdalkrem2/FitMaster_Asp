using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Queries.GetMembersList;

public class GetMembersListHandler(IApplicationDbContext db)
    : IRequestHandler<GetMembersListQuery, List<MemberListItemDto>>
{
    public async Task<List<MemberListItemDto>> Handle(GetMembersListQuery request, CancellationToken cancellationToken)
    {
        var query = db.MemberProfiles.Where(p => !p.Member.Deleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(p => p.Member.FullName.Contains(term) || p.Member.Phone.Contains(term));
        }

        return await query
            .OrderBy(p => p.Member.FullName)
            .Select(p => new MemberListItemDto(p.MemberId, p.Member.Phone, p.Member.FullName))
            .ToListAsync(cancellationToken);
    }
}
