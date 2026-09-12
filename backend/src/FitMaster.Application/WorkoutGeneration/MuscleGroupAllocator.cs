namespace FitMaster.Application.WorkoutGeneration;

/// <summary>
/// Expands a day's muscle-group priority weights into an ordered slot sequence of
/// exactly <c>totalSlots</c> entries, via weighted round-robin: each pass through
/// <paramref name="orderedGroups"/> gives one more slot to any group that hasn't yet
/// reached its weight, so a small total budget still yields "one of each
/// high-priority group" before any group gets a second slot - e.g. an Upper day
/// weighted Chest=3/Back=3/Shoulders=2/Triceps=2/Biceps=2 with only 5 slots
/// available gives Chest/Back/Shoulders/Triceps/Biceps (one each, the two lowest
/// -weighted groups don't get a repeat yet), not an arbitrary flat round robin
/// that treats every group as equally important.
///
/// Once every group has reached its weight and slots still remain (totalSlots
/// greater than the sum of weights), falls back to plain round-robin - ignoring
/// the weight cap - so a bigger day (e.g. an Advanced member's larger exercise
/// budget) still fills every slot instead of leaving some unfilled.
///
/// A group missing from <paramref name="weights"/> defaults to weight 1 - this is
/// what makes the allocator behave as plain round-robin when no weights are given
/// at all (every group weight-1), preserving the exact distribution
/// <see cref="ExerciseSelector"/> used before per-day-type weighting existed.
/// </summary>
public static class MuscleGroupAllocator
{
    public static IReadOnlyList<MuscleGroup> Allocate(
        IReadOnlyList<MuscleGroup> orderedGroups,
        IReadOnlyDictionary<MuscleGroup, int>? weights,
        int totalSlots)
    {
        if (orderedGroups.Count == 0 || totalSlots <= 0) return [];

        var result = new List<MuscleGroup>(totalSlots);
        var counts = orderedGroups.ToDictionary(g => g, _ => 0);
        var capsExhausted = false;

        while (result.Count < totalSlots)
        {
            var addedThisPass = false;
            foreach (var group in orderedGroups)
            {
                if (result.Count >= totalSlots) break;

                var cap = weights is not null && weights.TryGetValue(group, out var w) ? w : 1;
                if (capsExhausted || counts[group] < cap)
                {
                    result.Add(group);
                    counts[group]++;
                    addedThisPass = true;
                }
            }

            if (!addedThisPass)
            {
                // Every group already hit its weight cap but slots remain - switch to
                // plain round-robin (ignore caps) so the rest of the budget still gets spent.
                capsExhausted = true;
            }
        }

        return result;
    }
}
