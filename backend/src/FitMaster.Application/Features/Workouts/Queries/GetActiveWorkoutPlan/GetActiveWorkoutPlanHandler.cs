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
            .Select(w => new
            {
                w.Id,
                w.Name,
                w.MemberId,
                w.Goal,
                w.Level,
                w.SplitType,
                w.Status,
                w.CreatedAt,
                Days = w.WorkoutDays
                    .OrderBy(d => d.DayNumber)
                    .Select(d => new
                    {
                        d.Id,
                        d.DayNumber,
                        d.MuscleGroupLabel,
                        Exercises = d.WorkoutExercises
                            .OrderBy(e => e.OrderIndex)
                            .Select(e => new
                            {
                                e.Id,
                                e.ExerciseId,
                                Name = e.Exercise.Translations
                                    .Where(t => t.Locale == "en")
                                    .Select(t => t.Name)
                                    .FirstOrDefault(),
                                // JSON-array text; EF can only project the raw column,
                                // parsing happens after materializing below.
                                InstructionsJson = e.Exercise.Translations
                                    .Where(t => t.Locale == "en")
                                    .Select(t => t.Instructions)
                                    .FirstOrDefault(),
                                e.Exercise.DifficultyLevel,
                                PrimaryMuscle = e.Exercise.ExerciseMuscles
                                    .Where(em => em.Role == MuscleRole.Primary)
                                    .Select(em => em.Muscle.Name)
                                    .FirstOrDefault(),
                                TargetMuscles = e.Exercise.ExerciseMuscles.Select(em => em.Muscle.Name).ToList(),
                                Equipment = e.Exercise.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToList(),
                                ImageUrl = e.Exercise.Media.Select(m => m.MediaAsset.Url).FirstOrDefault(),
                                e.OrderIndex,
                                e.Sets,
                                e.Reps,
                                e.RepsMax,
                                e.DurationSeconds
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (plan is null) throw new NotFoundException(nameof(WorkoutPlan), request.MemberId);

        return new WorkoutPlanDto(
            plan.Id, plan.Name, plan.MemberId, plan.Goal, plan.Level, plan.SplitType, plan.Status, plan.CreatedAt,
            plan.Days.Select(d => new WorkoutDayDto(
                d.Id, d.DayNumber, d.MuscleGroupLabel,
                d.Exercises.Select(e => new WorkoutExerciseDto(
                    e.Id, e.ExerciseId, e.Name, e.DifficultyLevel, e.PrimaryMuscle, e.TargetMuscles, e.Equipment,
                    e.ImageUrl, e.OrderIndex, e.Sets, e.Reps, e.RepsMax, e.DurationSeconds,
                    WorkoutInstructionsParser.Parse(e.InstructionsJson)))
                    .ToList()))
                .ToList());
    }
}
