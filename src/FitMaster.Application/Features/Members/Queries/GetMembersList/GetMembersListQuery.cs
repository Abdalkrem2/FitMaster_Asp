using MediatR;

namespace FitMaster.Application.Features.Members.Queries.GetMembersList;

public record GetMembersListQuery(string? Search = null) : IRequest<List<MemberListItemDto>>;
