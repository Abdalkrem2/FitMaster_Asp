using MediatR;

namespace FitMaster.Application.Features.Members.Queries.GetMyProfile;

public record GetMyProfileQuery(long MemberId) : IRequest<MemberProfileDto>;
