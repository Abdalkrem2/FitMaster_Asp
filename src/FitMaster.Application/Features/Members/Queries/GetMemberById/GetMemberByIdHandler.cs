using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.Members;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Queries.GetMemberById;

public class GetMemberByIdHandler(IApplicationDbContext db) : IRequestHandler<GetMemberByIdQuery, MemberDto>
{
    public async Task<MemberDto> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await db.MemberProfiles
            .Where(p => p.MemberId == request.MemberId)
            .Select(p => new MemberDto(
                p.MemberId,
                p.Member.Phone,
                p.Member.FullName,
                p.Member.ProfilePicture,
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

        return member ?? throw new NotFoundException(nameof(MemberProfile), request.MemberId);
    }
}
