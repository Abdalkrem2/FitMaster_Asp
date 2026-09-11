using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Application.WorkoutGeneration;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Workouts.Commands.GenerateWorkoutPlan;

public class GenerateWorkoutPlanHandler(IApplicationDbContext db, IWorkoutPlanGenerator generator)
    : IRequestHandler<GenerateWorkoutPlanCommand, Result<long>>
{
    public async Task<Result<long>> Handle(GenerateWorkoutPlanCommand request, CancellationToken cancellationToken)
    {
        var profile = await db.MemberProfiles
            .FirstOrDefaultAsync(p => p.MemberId == request.MemberId, cancellationToken);
        if (profile is null)
        {
            return Result<long>.Failure("Member profile not found.");
        }
        if (profile.TrainingStyle is null)
        {
            return Result<long>.Failure("Set a training style on the member's profile before generating a workout plan.");
        }

        // Generating a new plan replaces the member's current one, same as renewing a membership.
        var activePlan = await db.WorkoutPlans
            .FirstOrDefaultAsync(w => w.MemberId == request.MemberId && w.Status == WorkoutPlanStatus.Active, cancellationToken);
        if (activePlan is not null)
        {
            activePlan.Status = WorkoutPlanStatus.Archived;
        }

        var plan = await generator.GenerateAsync(profile, profile.TrainingStyle.Value, cancellationToken);

        db.WorkoutPlans.Add(plan);
        await db.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(plan.Id);
    }
}
