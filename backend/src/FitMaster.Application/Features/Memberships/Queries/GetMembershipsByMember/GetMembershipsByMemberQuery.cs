using FitMaster.Application.Features.Memberships.Queries.GetMembershipById;
using MediatR;

namespace FitMaster.Application.Features.Memberships.Queries.GetMembershipsByMember;

public record GetMembershipsByMemberQuery(long MemberId) : IRequest<List<MembershipDto>>;
