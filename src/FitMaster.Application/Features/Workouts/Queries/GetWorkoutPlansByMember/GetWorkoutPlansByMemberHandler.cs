using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlansByMember;

public class GetWorkoutPlansByMemberHandler(IApplicationDbContext db)
    : IRequestHandler<GetWorkoutPlansByMemberQuery, List<WorkoutPlanDto>>
{
    public Task<List<WorkoutPlanDto>> Handle(GetWorkoutPlansByMemberQuery request, CancellationToken cancellationToken)
        => db.WorkoutPlans
            .Where(w => w.MemberId == request.MemberId)
            .OrderByDescending(w => w.CreatedAt)
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
            .ToListAsync(cancellationToken);
}
