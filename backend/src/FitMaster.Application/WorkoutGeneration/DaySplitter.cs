using FitMaster.Domain.Enums;

namespace FitMaster.Application.WorkoutGeneration;

/// <summary>
/// A single training day's label, the muscle groups it targets (in priority order -
/// matters when the day's exercise budget is smaller than the number of groups, see
/// <see cref="MuscleGroupAllocator"/>), and how many exercise slots each group should
/// get on a "full" day for that split. A group missing from <see cref="MuscleGroupWeights"/>
/// defaults to weight 1.
/// </summary>
public record DayTemplate(
    string Label,
    IReadOnlyList<MuscleGroup> TargetMuscleGroups,
    IReadOnlyDictionary<MuscleGroup, int>? MuscleGroupWeights = null);

/// <summary>Splits a <see cref="SplitType"/> into a fixed, sensible sequence of training days.</summary>
public interface IDaySplitter
{
    IReadOnlyList<DayTemplate> Split(SplitType splitType);
}

public class DaySplitter : IDaySplitter
{
    // --- Ordered by priority: when a day's exercise budget is too small to give every
    // group a slot, the lowest-priority groups at the end of the list are the ones that
    // get dropped for that session (see MuscleGroupAllocator) rather than an arbitrary one.
    //
    // Weights follow standard resistance-training programming convention: a muscle group
    // trained less often per week needs more volume concentrated in the session it does
    // get (Upper/Lower and Push/Pull/Legs splits train each muscle ~2x/week, so their days
    // carry noticeably more per-group volume), while Full Body trains everything ~3x/week
    // and so keeps each session light - one exercise per group, prioritizing the big
    // compound-pattern movers (squat, hinge, press, pull) over smaller synergists.

    private static readonly IReadOnlyList<MuscleGroup> FullBodyGroups =
        [MuscleGroup.Quadriceps, MuscleGroup.Back, MuscleGroup.Chest, MuscleGroup.Hamstrings, MuscleGroup.Shoulders, MuscleGroup.Glutes, MuscleGroup.Abs];

    private static readonly IReadOnlyDictionary<MuscleGroup, int> FullBodyWeights = new Dictionary<MuscleGroup, int>
    {
        [MuscleGroup.Quadriceps] = 1,
        [MuscleGroup.Back] = 1,
        [MuscleGroup.Chest] = 1,
        [MuscleGroup.Hamstrings] = 1,
        [MuscleGroup.Shoulders] = 1,
        [MuscleGroup.Glutes] = 1,
        [MuscleGroup.Abs] = 1,
    };

    // Upper: ~3 chest, ~3 back, ~2 shoulders, ~2 triceps, ~2 biceps - a standard
    // Upper/Lower hypertrophy-day allocation (chest/back get the most volume as the
    // largest pressing/pulling groups, shoulders/arms as secondary movers and isolation).
    private static readonly IReadOnlyList<MuscleGroup> UpperGroups =
        [MuscleGroup.Chest, MuscleGroup.Back, MuscleGroup.Shoulders, MuscleGroup.Triceps, MuscleGroup.Biceps];

    private static readonly IReadOnlyDictionary<MuscleGroup, int> UpperWeights = new Dictionary<MuscleGroup, int>
    {
        [MuscleGroup.Chest] = 3,
        [MuscleGroup.Back] = 3,
        [MuscleGroup.Shoulders] = 2,
        [MuscleGroup.Triceps] = 2,
        [MuscleGroup.Biceps] = 2,
    };

    // Lower: ~2 quads, ~2 hamstrings, ~2 glutes, ~1 calves.
    private static readonly IReadOnlyList<MuscleGroup> LowerGroups =
        [MuscleGroup.Quadriceps, MuscleGroup.Hamstrings, MuscleGroup.Glutes, MuscleGroup.Calves];

    private static readonly IReadOnlyDictionary<MuscleGroup, int> LowerWeights = new Dictionary<MuscleGroup, int>
    {
        [MuscleGroup.Quadriceps] = 2,
        [MuscleGroup.Hamstrings] = 2,
        [MuscleGroup.Glutes] = 2,
        [MuscleGroup.Calves] = 1,
    };

    // Push: chest-dominant (~3), shoulders and triceps as secondary pressing movers (~2 each).
    private static readonly IReadOnlyList<MuscleGroup> PushGroups =
        [MuscleGroup.Chest, MuscleGroup.Shoulders, MuscleGroup.Triceps];

    private static readonly IReadOnlyDictionary<MuscleGroup, int> PushWeights = new Dictionary<MuscleGroup, int>
    {
        [MuscleGroup.Chest] = 3,
        [MuscleGroup.Shoulders] = 2,
        [MuscleGroup.Triceps] = 2,
    };

    // Pull: back-dominant (~3, it's a large muscle group carrying most of a pull day's
    // volume), biceps as the secondary pulling mover (~2).
    private static readonly IReadOnlyList<MuscleGroup> PullGroups =
        [MuscleGroup.Back, MuscleGroup.Biceps];

    private static readonly IReadOnlyDictionary<MuscleGroup, int> PullWeights = new Dictionary<MuscleGroup, int>
    {
        [MuscleGroup.Back] = 3,
        [MuscleGroup.Biceps] = 2,
    };

    // Legs (PPL): ~3 quads, ~2 hamstrings, ~1 glutes, ~1 calves - quad-dominant since Legs
    // day here carries the squat-pattern volume that Push/Pull don't otherwise cover.
    private static readonly IReadOnlyList<MuscleGroup> LegsGroups =
        [MuscleGroup.Quadriceps, MuscleGroup.Hamstrings, MuscleGroup.Glutes, MuscleGroup.Calves];

    private static readonly IReadOnlyDictionary<MuscleGroup, int> LegsWeights = new Dictionary<MuscleGroup, int>
    {
        [MuscleGroup.Quadriceps] = 3,
        [MuscleGroup.Hamstrings] = 2,
        [MuscleGroup.Glutes] = 1,
        [MuscleGroup.Calves] = 1,
    };

    public IReadOnlyList<DayTemplate> Split(SplitType splitType) => splitType switch
    {
        SplitType.FullBody =>
        [
            new DayTemplate("Full Body", FullBodyGroups, FullBodyWeights),
            new DayTemplate("Full Body", FullBodyGroups, FullBodyWeights),
            new DayTemplate("Full Body", FullBodyGroups, FullBodyWeights),
        ],

        SplitType.UpperLower =>
        [
            new DayTemplate("Upper Body", UpperGroups, UpperWeights),
            new DayTemplate("Lower Body", LowerGroups, LowerWeights),
            new DayTemplate("Upper Body", UpperGroups, UpperWeights),
            new DayTemplate("Lower Body", LowerGroups, LowerWeights),
        ],

        SplitType.PushPullLegs =>
        [
            new DayTemplate("Push", PushGroups, PushWeights),
            new DayTemplate("Pull", PullGroups, PullWeights),
            new DayTemplate("Legs", LegsGroups, LegsWeights),
            new DayTemplate("Push", PushGroups, PushWeights),
            new DayTemplate("Pull", PullGroups, PullWeights),
            new DayTemplate("Legs", LegsGroups, LegsWeights),
        ],

        _ => throw new ArgumentOutOfRangeException(nameof(splitType), splitType, null),
    };
}
