using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Commands.UpdateMemberProfile;

public class UpdateMemberProfileHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateMemberProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateMemberProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await db.MemberProfiles
            .FirstOrDefaultAsync(p => p.MemberId == request.MemberId, cancellationToken);

        if (profile is null)
        {
            return Result.Failure("Member profile not found.");
        }

        profile.Goal = request.Goal;
        profile.FitnessLevel = request.FitnessLevel;
        profile.SplitType = request.SplitType;
        profile.Injuries = request.Injuries ?? [];
        profile.Weight = request.Weight;
        profile.Height = request.Height;
        profile.Age = request.Age;
        profile.HasDiabetes = request.HasDiabetes;
        profile.HasHeartConditions = request.HasHeartConditions;
        profile.HasHypertension = request.HasHypertension;
        profile.Allergies = request.Allergies ?? [];

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
