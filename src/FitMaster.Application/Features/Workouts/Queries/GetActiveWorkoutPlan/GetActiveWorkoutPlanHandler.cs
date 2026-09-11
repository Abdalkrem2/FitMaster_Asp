using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;
using FitMaster.Domain.Entities.Workouts;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Workouts.Queries.GetActiveWorkoutPlan;

public class GetActiveWorkoutPlanHandler(IApplicationDbContext db) : IRequestHandler<GetActiveWorkoutPlanQuery, WorkoutPlanDto>
{
    public async Task<WorkoutPlanDto> Handle(GetActiveWorkoutPlanQuery request, CancellationToken cancellationToken)
    {
        var plan = await db.WorkoutPlans
            .Where(w => w.MemberId == request.MemberId && w.Status == WorkoutPlanStatus.Active)
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
                                e.Exercise.DifficultyLevel,
                                e.Exercise.ExerciseMuscles
                                    .Where(em => em.Role == MuscleRole.Primary)
                                    .Select(em => em.Muscle.Name)
                                    .FirstOrDefault(),
                                e.Exercise.ExerciseMuscles.Select(em => em.Muscle.Name).ToList(),
                                e.Exercise.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToList(),
                                e.Exercise.Media.Select(m => m.MediaAsset.Url).FirstOrDefault(),
                                e.OrderIndex,
                                e.Sets,
                                e.Reps,
                                e.RepsMax,
                                e.DurationSeconds))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        return plan ?? throw new NotFoundException(nameof(WorkoutPlan), request.MemberId);
    }
}
