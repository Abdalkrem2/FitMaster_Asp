using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Workouts.Commands.RenameWorkoutPlan;

public class RenameWorkoutPlanHandler(IApplicationDbContext db) : IRequestHandler<RenameWorkoutPlanCommand, Result>
{
    public async Task<Result> Handle(RenameWorkoutPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await db.WorkoutPlans.FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);
        if (plan is null)
        {
            return Result.Failure("Workout plan not found.");
        }

        plan.Name = request.Name;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
