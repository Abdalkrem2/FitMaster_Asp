using MediatR;

namespace FitMaster.Application.Features.Members.Queries.GetMemberById;

public record GetMemberByIdQuery(long MemberId) : IRequest<MemberDto>;
