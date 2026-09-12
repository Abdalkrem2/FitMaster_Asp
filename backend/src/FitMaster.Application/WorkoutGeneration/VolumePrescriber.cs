using FitMaster.Domain.Enums;

namespace FitMaster.Application.WorkoutGeneration;

public record VolumePrescription(int Sets, int RepsMin, int RepsMax);

/// <summary>Sets/reps and per-day exercise count, driven by TrainingStyle and FitnessLevel.</summary>
public interface IVolumePrescriber
{
    VolumePrescription Prescribe(TrainingStyle style, FitnessLevel level);

    /// <summary>Total exercises to prescribe for one training day.</summary>
    int ExerciseCountForDay(FitnessLevel level);
}

public class VolumePrescriber : IVolumePrescriber
{
    public VolumePrescription Prescribe(TrainingStyle style, FitnessLevel level)
    {
        var sets = ResolveSets(style, level);

        return style switch
        {
            TrainingStyle.Strength => new VolumePrescription(sets, 3, 6),
            TrainingStyle.Hypertrophy => new VolumePrescription(sets, 8, 12),
            TrainingStyle.Circuit => new VolumePrescription(sets, 15, 20),
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null),
        };
    }

    public int ExerciseCountForDay(FitnessLevel level) => level switch
    {
        FitnessLevel.Beginner => 4,
        FitnessLevel.Intermediate => 5,
        FitnessLevel.Advanced => 6,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
    };

    private static int ResolveSets(TrainingStyle style, FitnessLevel level) => style switch
    {
        TrainingStyle.Strength => level switch
        {
            FitnessLevel.Beginner => 3,
            FitnessLevel.Intermediate => 4,
            FitnessLevel.Advanced => 5,
            _ => 3,
        },
        TrainingStyle.Hypertrophy => level == FitnessLevel.Beginner ? 3 : 4,
        TrainingStyle.Circuit => level == FitnessLevel.Beginner ? 2 : 3,
        _ => throw new ArgumentOutOfRangeException(nameof(style), style, null),
    };
}
