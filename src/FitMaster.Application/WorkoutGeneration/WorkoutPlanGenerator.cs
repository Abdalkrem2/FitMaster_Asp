using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Entities.Workouts;
using FitMaster.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.WorkoutGeneration;

/// <summary>
/// Builds a full <see cref="WorkoutPlan"/> (days + exercises) for a member, from real
/// seeded exercise data. Rule-based and deterministic by design - see the handoff notes
/// for why this isn't AI-driven: plan generation needs to stay fast, testable, and free
/// of external-API dependencies. Composed of three independently-testable pieces:
/// <see cref="IDaySplitter"/>, <see cref="IExerciseSelector"/>, <see cref="IVolumePrescriber"/>.
/// Does not save - the caller (GenerateWorkoutPlanHandler) owns persistence.
/// </summary>
public interface IWorkoutPlanGenerator
{
    Task<WorkoutPlan> GenerateAsync(MemberProfile profile, TrainingStyle trainingStyle, CancellationToken cancellationToken);
}

public class WorkoutPlanGenerator(
    IApplicationDbContext db,
    IDaySplitter daySplitter,
    IExerciseSelector exerciseSelector,
    IVolumePrescriber volumePrescriber) : IWorkoutPlanGenerator
{
    public async Task<WorkoutPlan> GenerateAsync(MemberProfile profile, TrainingStyle trainingStyle, CancellationToken cancellationToken)
    {
        var muscleGroupByMuscleId = await LoadMuscleGroupsAsync(cancellationToken);
        var injuryExcludedMuscleIds = await LoadInjuryExclusionsAsync(profile.Injuries, cancellationToken);
        var candidates = await LoadExerciseCandidatesAsync(muscleGroupByMuscleId, cancellationToken);

        var days = daySplitter.Split(profile.SplitType);
        var exerciseCount = volumePrescriber.ExerciseCountForDay(profile.FitnessLevel);
        var volume = volumePrescriber.Prescribe(trainingStyle, profile.FitnessLevel);
        var usedExerciseIds = new HashSet<Guid>();

        var plan = new WorkoutPlan
        {
            Name = $"{profile.SplitType} - {profile.Goal}",
            Goal = profile.Goal,
            Level = profile.FitnessLevel,
            Status = WorkoutPlanStatus.Active,
            SplitType = profile.SplitType,
            MemberId = profile.MemberId,
            CreatedAt = DateTime.UtcNow,
        };

        var dayNumber = 1;
        foreach (var dayTemplate in days)
        {
            var selectedExerciseIds = exerciseSelector.SelectForDay(
                dayTemplate, candidates, profile.FitnessLevel, injuryExcludedMuscleIds, usedExerciseIds, exerciseCount);

            var workoutDay = new WorkoutDay
            {
                WorkoutPlanId = 0, // set by EF via the owning collection
                MuscleGroupLabel = dayTemplate.Label,
                DayNumber = dayNumber++,
            };

            var order = 1;
            foreach (var exerciseId in selectedExerciseIds)
            {
                workoutDay.WorkoutExercises.Add(new WorkoutExercise
                {
                    WorkoutDayId = 0, // set by EF via the owning collection
                    ExerciseId = exerciseId,
                    OrderIndex = order++,
                    Sets = volume.Sets,
                    Reps = volume.RepsMin,
                    RepsMax = volume.RepsMax,
                });
            }

            plan.WorkoutDays.Add(workoutDay);
        }

        return plan;
    }

    private async Task<IReadOnlyDictionary<long, MuscleGroup>> LoadMuscleGroupsAsync(CancellationToken cancellationToken)
    {
        var allSynonyms = MuscleGroupCatalog.SynonymsByGroup
            .SelectMany(kv => kv.Value.Select(name => (Group: kv.Key, Name: name)))
            .ToList();
        var synonymNames = allSynonyms.Select(x => x.Name).ToArray();

        var muscles = await db.Muscles
            .Where(m => synonymNames.Contains(m.Name))
            .Select(m => new { m.Id, m.Name })
            .ToListAsync(cancellationToken);

        var groupByName = allSynonyms.ToDictionary(x => x.Name, x => x.Group);

        return muscles
            .Where(m => groupByName.ContainsKey(m.Name))
            .ToDictionary(m => m.Id, m => groupByName[m.Name]);
    }

    private async Task<IReadOnlySet<long>> LoadInjuryExclusionsAsync(List<InjuryType> injuries, CancellationToken cancellationToken)
    {
        if (injuries.Count == 0) return new HashSet<long>();

        var muscleIds = await db.InjuryMuscleExclusions
            .Where(x => injuries.Contains(x.InjuryType))
            .Select(x => x.MuscleId)
            .ToListAsync(cancellationToken);

        return muscleIds.ToHashSet();
    }

    private async Task<IReadOnlyList<ExerciseCandidate>> LoadExerciseCandidatesAsync(
        IReadOnlyDictionary<long, MuscleGroup> muscleGroupByMuscleId, CancellationToken cancellationToken)
    {
        var raw = await db.Exercises
            .Where(e => !e.IsArchived)
            .Where(e => e.Tags.Any(t => t.Category == TagCategory.Category && t.Name == "strength"))
            .Select(e => new
            {
                e.Id,
                e.DifficultyLevel,
                Muscles = e.ExerciseMuscles.Select(em => new { em.MuscleId, em.Role }).ToList(),
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return raw
            .Select(e => new ExerciseCandidate(
                e.Id,
                e.DifficultyLevel,
                e.Muscles
                    .Select(m => new MuscleTarget(
                        m.MuscleId,
                        muscleGroupByMuscleId.TryGetValue(m.MuscleId, out var group) ? group : null,
                        m.Role))
                    .ToList()))
            .ToList();
    }
}
