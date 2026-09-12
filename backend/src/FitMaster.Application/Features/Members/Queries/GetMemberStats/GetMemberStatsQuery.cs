using MediatR;

namespace FitMaster.Application.Features.Members.Queries.GetMemberStats;

public record GetMemberStatsQuery : IRequest<MemberStatsDto>;
