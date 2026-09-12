using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Nutrition.Commands.ArchiveNutritionPlan;

public class ArchiveNutritionPlanHandler(IApplicationDbContext db) : IRequestHandler<ArchiveNutritionPlanCommand, Result>
{
    public async Task<Result> Handle(ArchiveNutritionPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await db.NutritionPlans.FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);
        if (plan is null)
        {
            return Result.Failure("Nutrition plan not found.");
        }

        plan.Status = WorkoutPlanStatus.Archived;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
