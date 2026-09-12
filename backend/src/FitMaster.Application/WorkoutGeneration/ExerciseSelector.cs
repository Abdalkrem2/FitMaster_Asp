using FitMaster.Domain.Enums;

namespace FitMaster.Application.WorkoutGeneration;

/// <summary>
/// Picks exercises for one training day from a candidate pool, following the day's
/// muscle-group allocation (see <see cref="MuscleGroupAllocator"/>) so the result looks
/// like a real, recognizable program structure - e.g. an Upper day getting noticeably
/// more chest/back exercises than biceps/triceps - rather than a flat, unstructured
/// round robin across every targeted muscle. Pure/data-only - no EF Core dependency, so
/// it can be unit tested with hand-built <see cref="ExerciseCandidate"/> lists.
/// </summary>
public interface IExerciseSelector
{
    /// <summary>
    /// Selects up to <paramref name="exerciseCount"/> exercises for the day, one per
    /// allocated muscle-group slot (compound movements preferred within each slot,
    /// falling back to isolation if no compound is eligible for that group). Exercises
    /// whose primary target muscle is in <paramref name="injuryExcludedMuscleIds"/> are
    /// never selected; exercises where only a secondary muscle is excluded are
    /// de-prioritized rather than dropped. <see cref="AdvancedMovementBlocklist"/>
    /// exercises are always excluded, for every fitness level - these are specialist
    /// skill-training movements (planche, muscle-ups, ...) that don't belong in an
    /// auto-generated plan regardless of how advanced the member is. When
    /// <paramref name="equipmentPreference"/> is Bodyweight, any exercise with at least
    /// one required piece of equipment is excluded too. Selected ids are added to
    /// <paramref name="usedExerciseIds"/> so later days in the same plan don't repeat them.
    /// </summary>
    IReadOnlyList<Guid> SelectForDay(
        DayTemplate day,
        IReadOnlyList<ExerciseCandidate> candidates,
        FitnessLevel fitnessLevel,
        IReadOnlySet<long> injuryExcludedMuscleIds,
        HashSet<Guid> usedExerciseIds,
        int exerciseCount,
        EquipmentPreference equipmentPreference = EquipmentPreference.Gym);
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
        int exerciseCount,
        EquipmentPreference equipmentPreference = EquipmentPreference.Gym)
    {
        var eligible = candidates
            .Where(c => !usedExerciseIds.Contains(c.ExerciseId))
            .Where(c => IsWithinDifficulty(c.Difficulty, fitnessLevel))
            .Where(c => !AdvancedMovementBlocklist.IsAdvancedMovement(c.Name))
            .Where(c => equipmentPreference != EquipmentPreference.Bodyweight || !c.RequiresEquipment)
            .Where(c => !HasExcludedMuscle(c, injuryExcludedMuscleIds, MuscleRole.Primary))
            .Select(c => new RankedCandidate(c, HasExcludedMuscle(c, injuryExcludedMuscleIds, MuscleRole.Secondary)))
            .ToList();

        var selected = new List<Guid>();

        var allocation = MuscleGroupAllocator.Allocate(day.TargetMuscleGroups, day.MuscleGroupWeights, exerciseCount);

        foreach (var group in allocation)
        {
            if (selected.Count >= exerciseCount) break;

            var pick = eligible
                .Where(x => !selected.Contains(x.Candidate.ExerciseId))
                .Where(x => TargetsGroupAsPrimary(x.Candidate, group))
                .OrderByDescending(x => x.Candidate.CompoundScore >= CompoundMuscleThreshold)
                .ThenBy(x => x.InjuryPenalty)
                .ThenByDescending(x => x.Candidate.CompoundScore)
                .FirstOrDefault();

            if (pick is not null) selected.Add(pick.Candidate.ExerciseId);
        }

        // Data-scarcity fallback: if a specific group ran out of eligible candidates
        // before its allocated slots were used, spend the remaining budget on any other
        // still-eligible candidate for the day rather than under-filling the session.
        if (selected.Count < exerciseCount)
        {
            var backfill = eligible
                .Where(x => !selected.Contains(x.Candidate.ExerciseId))
                .Where(x => day.TargetMuscleGroups.Any(g => TargetsGroupAsPrimary(x.Candidate, g)))
                .OrderBy(x => x.InjuryPenalty)
                .ThenByDescending(x => x.Candidate.CompoundScore);

            foreach (var candidate in backfill)
            {
                if (selected.Count >= exerciseCount) break;
                selected.Add(candidate.Candidate.ExerciseId);
            }
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
        // classification improves. AdvancedMovementBlocklist (applied above,
        // unconditionally) is what actually keeps true elite skill moves out for
        // everyone - this tier check alone can't do that given the uniform data.
        return (int)exerciseDifficulty.Value <= (int)memberLevel + 1;
    }

    private sealed record RankedCandidate(ExerciseCandidate Candidate, bool InjuryPenalty);
}
