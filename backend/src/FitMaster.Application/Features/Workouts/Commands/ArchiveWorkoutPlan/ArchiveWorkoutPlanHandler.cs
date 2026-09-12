using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Workouts.Commands.ArchiveWorkoutPlan;

public class ArchiveWorkoutPlanHandler(IApplicationDbContext db) : IRequestHandler<ArchiveWorkoutPlanCommand, Result>
{
    public async Task<Result> Handle(ArchiveWorkoutPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await db.WorkoutPlans.FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);
        if (plan is null)
        {
            return Result.Failure("Workout plan not found.");
        }

        plan.Status = WorkoutPlanStatus.Archived;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
