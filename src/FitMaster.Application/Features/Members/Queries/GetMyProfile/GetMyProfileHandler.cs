using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.Members;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Queries.GetMyProfile;

public class GetMyProfileHandler(IApplicationDbContext db) : IRequestHandler<GetMyProfileQuery, MemberProfileDto>
{
    public async Task<MemberProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await db.MemberProfiles
            .Where(p => p.MemberId == request.MemberId)
            .Select(p => new MemberProfileDto(
                p.Goal,
                p.FitnessLevel,
                p.SplitType,
                p.TrainingStyle,
                p.Injuries,
                p.Weight,
                p.Height,
                p.Age,
                p.HasDiabetes,
                p.HasHeartConditions,
                p.HasHypertension,
                p.Allergies))
            .FirstOrDefaultAsync(cancellationToken);

        return profile ?? throw new NotFoundException(nameof(MemberProfile), request.MemberId);
    }
}
