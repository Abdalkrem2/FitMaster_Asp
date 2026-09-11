using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.Workouts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;

public class GetWorkoutPlanByIdHandler(IApplicationDbContext db) : IRequestHandler<GetWorkoutPlanByIdQuery, WorkoutPlanDto>
{
    public async Task<WorkoutPlanDto> Handle(GetWorkoutPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var plan = await db.WorkoutPlans
            .Where(w => w.Id == request.Id)
            .Select(w => new WorkoutPlanDto(
                w.Id,
                w.Name,
                w.MemberId,
                w.Goal,
                w.Level,
                w.SplitType,
                w.Status,
                w.CreatedAt,
                w.WorkoutDays
                    .OrderBy(d => d.DayNumber)
                    .Select(d => new WorkoutDayDto(
                        d.Id,
                        d.DayNumber,
                        d.MuscleGroupLabel,
                        d.WorkoutExercises
                            .OrderBy(e => e.OrderIndex)
                            .Select(e => new WorkoutExerciseDto(
                                e.Id,
                                e.ExerciseId,
                                e.Exercise.Translations
                                    .Where(t => t.Locale == "en")
                                    .Select(t => t.Name)
                                    .FirstOrDefault(),
                                e.OrderIndex,
                                e.Sets,
                                e.Reps,
                                e.RepsMax,
                                e.DurationSeconds))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        return plan ?? throw new NotFoundException(nameof(WorkoutPlan), request.Id);
    }
}
