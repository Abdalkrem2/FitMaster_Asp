using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Application.NutritionGeneration;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Nutrition.Commands.GenerateNutritionPlan;

public class GenerateNutritionPlanHandler(IApplicationDbContext db, INutritionPlanGenerator generator)
    : IRequestHandler<GenerateNutritionPlanCommand, Result<long>>
{
    public async Task<Result<long>> Handle(GenerateNutritionPlanCommand request, CancellationToken cancellationToken)
    {
        var profile = await db.MemberProfiles
            .Include(p => p.Member)
            .FirstOrDefaultAsync(p => p.MemberId == request.MemberId, cancellationToken);
        if (profile is null)
        {
            return Result<long>.Failure("Member profile not found.");
        }
        if (profile.Weight is null || profile.Height is null || profile.Age is null)
        {
            return Result<long>.Failure("Set weight, height, and age on the member's profile before generating a nutrition plan.");
        }

        // Generating a new plan replaces the member's current one, same as workout plans.
        var activePlan = await db.NutritionPlans
            .FirstOrDefaultAsync(n => n.MemberId == request.MemberId && n.Status == WorkoutPlanStatus.Active, cancellationToken);
        if (activePlan is not null)
        {
            activePlan.Status = WorkoutPlanStatus.Archived;
        }

        var plan = await generator.GenerateAsync(profile, profile.Member.Gender, cancellationToken);

        db.NutritionPlans.Add(plan);
        await db.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(plan.Id);
    }
}
