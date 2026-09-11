using MediatR;

namespace FitMaster.Application.Features.Memberships.Queries.GetMembershipById;

public record GetMembershipByIdQuery(long Id) : IRequest<MembershipDto>;
