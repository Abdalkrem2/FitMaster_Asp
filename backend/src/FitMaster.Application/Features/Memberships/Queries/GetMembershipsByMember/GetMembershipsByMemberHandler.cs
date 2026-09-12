using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Memberships.Queries.GetMembershipById;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Memberships.Queries.GetMembershipsByMember;

public class GetMembershipsByMemberHandler(IApplicationDbContext db)
    : IRequestHandler<GetMembershipsByMemberQuery, List<MembershipDto>>
{
    public async Task<List<MembershipDto>> Handle(GetMembershipsByMemberQuery request, CancellationToken cancellationToken)
    {
        return await db.Memberships
            .Where(m => m.MemberId == request.MemberId)
            .OrderByDescending(m => m.StartDate)
            .Select(m => new MembershipDto(
                m.Id,
                m.MemberId,
                m.Member.FullName,
                m.PackageId,
                m.Package.Name,
                m.Status,
                m.StartDate,
                m.EndDate,
                m.Price,
                m.Debt,
                m.Description))
            .ToListAsync(cancellationToken);
    }
}
