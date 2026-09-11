using FitMaster.Domain.Enums;

namespace FitMaster.Application.WorkoutGeneration;

/// <summary>A single training day's label and the muscle groups it targets.</summary>
public record DayTemplate(string Label, IReadOnlyList<MuscleGroup> TargetMuscleGroups);

/// <summary>Splits a <see cref="SplitType"/> into a fixed, sensible sequence of training days.</summary>
public interface IDaySplitter
{
    IReadOnlyList<DayTemplate> Split(SplitType splitType);
}

public class DaySplitter : IDaySplitter
{
    private static readonly IReadOnlyList<MuscleGroup> FullBodyGroups =
        [MuscleGroup.Chest, MuscleGroup.Back, MuscleGroup.Shoulders, MuscleGroup.Quadriceps, MuscleGroup.Hamstrings, MuscleGroup.Glutes, MuscleGroup.Abs];

    private static readonly IReadOnlyList<MuscleGroup> UpperGroups =
        [MuscleGroup.Chest, MuscleGroup.Back, MuscleGroup.Shoulders, MuscleGroup.Biceps, MuscleGroup.Triceps];

    private static readonly IReadOnlyList<MuscleGroup> LowerGroups =
        [MuscleGroup.Quadriceps, MuscleGroup.Hamstrings, MuscleGroup.Glutes, MuscleGroup.Calves];

    private static readonly IReadOnlyList<MuscleGroup> PushGroups =
        [MuscleGroup.Chest, MuscleGroup.Shoulders, MuscleGroup.Triceps];

    private static readonly IReadOnlyList<MuscleGroup> PullGroups =
        [MuscleGroup.Back, MuscleGroup.Biceps];

    private static readonly IReadOnlyList<MuscleGroup> LegsGroups =
        [MuscleGroup.Quadriceps, MuscleGroup.Hamstrings, MuscleGroup.Glutes, MuscleGroup.Calves];

    public IReadOnlyList<DayTemplate> Split(SplitType splitType) => splitType switch
    {
        SplitType.FullBody =>
        [
            new DayTemplate("Full Body", FullBodyGroups),
            new DayTemplate("Full Body", FullBodyGroups),
            new DayTemplate("Full Body", FullBodyGroups),
        ],

        SplitType.UpperLower =>
        [
            new DayTemplate("Upper Body", UpperGroups),
            new DayTemplate("Lower Body", LowerGroups),
            new DayTemplate("Upper Body", UpperGroups),
            new DayTemplate("Lower Body", LowerGroups),
        ],

        SplitType.PushPullLegs =>
        [
            new DayTemplate("Push", PushGroups),
            new DayTemplate("Pull", PullGroups),
            new DayTemplate("Legs", LegsGroups),
            new DayTemplate("Push", PushGroups),
            new DayTemplate("Pull", PullGroups),
            new DayTemplate("Legs", LegsGroups),
        ],

        _ => throw new ArgumentOutOfRangeException(nameof(splitType), splitType, null),
    };
}
