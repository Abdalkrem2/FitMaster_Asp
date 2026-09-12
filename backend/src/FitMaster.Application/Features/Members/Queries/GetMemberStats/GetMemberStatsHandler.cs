using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Queries.GetMemberStats;

public class GetMemberStatsHandler(IApplicationDbContext db) : IRequestHandler<GetMemberStatsQuery, MemberStatsDto>
{
    private const int ExpiringSoonWithinDays = 7;

    public async Task<MemberStatsDto> Handle(GetMemberStatsQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var soon = today.AddDays(ExpiringSoonWithinDays);

        var activeMembers = await db.Memberships
            .Where(m => m.Status == MembershipStatus.Active)
            .Select(m => m.MemberId)
            .Distinct()
            .CountAsync(cancellationToken);

        var expiringSoon = await db.Memberships
            .Where(m => m.Status == MembershipStatus.Active && m.EndDate >= today && m.EndDate <= soon)
            .Select(m => m.MemberId)
            .Distinct()
            .CountAsync(cancellationToken);

        return new MemberStatsDto(activeMembers, expiringSoon);
    }
}
