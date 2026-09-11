using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Members.Queries.GetMembersList;

public record GetMembersListQuery(string? Search = null, int Page = 0, int Size = 20) : IRequest<PagedResult<MemberListItemDto>>;
