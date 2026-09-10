using FitMaster.Domain.Enums;

namespace FitMaster.Application.WorkoutGeneration;

/// <summary>
/// Picks exercises for one training day from a candidate pool: compound
/// movements first (one per target muscle group), then isolation exercises
/// to fill the remaining budget. Pure/data-only - no EF Core dependency, so
/// it can be unit tested with hand-built <see cref="ExerciseCandidate"/> lists.
/// </summary>
public interface IExerciseSelector
{
    /// <summary>
    /// Selects up to <paramref name="exerciseCount"/> exercises for the day. Exercises
    /// whose primary target muscle is in <paramref name="injuryExcludedMuscleIds"/> are
    /// never selected; exercises where only a secondary muscle is excluded are
    /// de-prioritized rather than dropped. Selected ids are added to
    /// <paramref name="usedExerciseIds"/> so later days in the same plan don't repeat them.
    /// </summary>
    IReadOnlyList<Guid> SelectForDay(
        DayTemplate day,
        IReadOnlyList<ExerciseCandidate> candidates,
        FitnessLevel fitnessLevel,
        IReadOnlySet<long> injuryExcludedMuscleIds,
        HashSet<Guid> usedExerciseIds,
        int exerciseCount);
}

public class ExerciseSelector : IExerciseSelector
{
    private const int CompoundMuscleThreshold = 4;

    public IReadOnlyList<Guid> SelectForDay(
        DayTemplate day,
        IReadOnlyList<ExerciseCandidate> candidates,
        FitnessLevel fitnessLevel,
        IReadOnlySet<long> injuryExcludedMuscleIds,
        HashSet<Guid> usedExerciseIds,
        int exerciseCount)
    {
        var eligible = candidates
            .Where(c => !usedExerciseIds.Contains(c.ExerciseId))
            .Where(c => IsWithinDifficulty(c.Difficulty, fitnessLevel))
            .Where(c => !HasExcludedMuscle(c, injuryExcludedMuscleIds, MuscleRole.Primary))
            .Select(c => new RankedCandidate(c, HasExcludedMuscle(c, injuryExcludedMuscleIds, MuscleRole.Secondary)))
            .ToList();

        var selected = new List<Guid>();

        // Compound movements first: one per target group, in day order.
        foreach (var group in day.TargetMuscleGroups)
        {
            if (selected.Count >= exerciseCount) break;

            var pick = eligible
                .Where(x => !selected.Contains(x.Candidate.ExerciseId))
                .Where(x => TargetsGroupAsPrimary(x.Candidate, group))
                .Where(x => x.Candidate.CompoundScore >= CompoundMuscleThreshold)
                .OrderBy(x => x.InjuryPenalty)
                .ThenByDescending(x => x.Candidate.CompoundScore)
                .FirstOrDefault();

            if (pick is not null) selected.Add(pick.Candidate.ExerciseId);
        }

        // Fill remaining slots with isolation exercises, cycling through the target groups.
        var attempts = 0;
        var maxAttempts = day.TargetMuscleGroups.Count * Math.Max(exerciseCount, 1);
        while (selected.Count < exerciseCount && attempts < maxAttempts && day.TargetMuscleGroups.Count > 0)
        {
            var group = day.TargetMuscleGroups[attempts % day.TargetMuscleGroups.Count];
            attempts++;

            var pick = eligible
                .Where(x => !selected.Contains(x.Candidate.ExerciseId))
                .Where(x => TargetsGroupAsPrimary(x.Candidate, group))
                .OrderBy(x => x.InjuryPenalty)
                .ThenByDescending(x => x.Candidate.CompoundScore)
                .FirstOrDefault();

            if (pick is not null) selected.Add(pick.Candidate.ExerciseId);
        }

        foreach (var id in selected) usedExerciseIds.Add(id);
        return selected;
    }

    private static bool TargetsGroupAsPrimary(ExerciseCandidate candidate, MuscleGroup group)
        => candidate.Targets.Any(t => t.Group == group && t.Role == MuscleRole.Primary);

    private static bool HasExcludedMuscle(ExerciseCandidate candidate, IReadOnlySet<long> excludedMuscleIds, MuscleRole role)
        => candidate.Targets.Any(t => t.Role == role && excludedMuscleIds.Contains(t.MuscleId));

    private static bool IsWithinDifficulty(DifficultyLevel? exerciseDifficulty, FitnessLevel memberLevel)
    {
        // Unclassified exercises (no difficulty tag in the source data) stay eligible.
        if (exerciseDifficulty is null) return true;

        // FitnessLevel and DifficultyLevel share the same Beginner/Intermediate/Advanced
        // ordering. Allow one tier above the member's own level: the seeded exercise
        // catalog currently has every exercise classified as Intermediate (no real
        // Beginner/Advanced variety yet), so a strict "own level or easier" filter
        // would leave Beginner members with zero eligible exercises. One tier of
        // leniency keeps Beginners out of Advanced-only work while staying usable
        // against today's data, and still tightens correctly once difficulty
        // classification improves.
        return (int)exerciseDifficulty.Value <= (int)memberLevel + 1;
    }

    private sealed record RankedCandidate(ExerciseCandidate Candidate, bool InjuryPenalty);
}
