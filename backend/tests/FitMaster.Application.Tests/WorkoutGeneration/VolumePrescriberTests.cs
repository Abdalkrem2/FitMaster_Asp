using FitMaster.Application.WorkoutGeneration;
using FitMaster.Domain.Enums;

namespace FitMaster.Application.Tests.WorkoutGeneration;

public class VolumePrescriberTests
{
    private readonly VolumePrescriber _sut = new();

    [Theory]
    [InlineData(TrainingStyle.Strength, 3, 6)]
    [InlineData(TrainingStyle.Hypertrophy, 8, 12)]
    [InlineData(TrainingStyle.Circuit, 15, 20)]
    public void Prescribe_uses_the_rep_range_for_the_training_style(TrainingStyle style, int expectedMin, int expectedMax)
    {
        var prescription = _sut.Prescribe(style, FitnessLevel.Intermediate);

        Assert.Equal(expectedMin, prescription.RepsMin);
        Assert.Equal(expectedMax, prescription.RepsMax);
    }

    [Theory]
    [InlineData(TrainingStyle.Strength, FitnessLevel.Beginner, 3)]
    [InlineData(TrainingStyle.Strength, FitnessLevel.Intermediate, 4)]
    [InlineData(TrainingStyle.Strength, FitnessLevel.Advanced, 5)]
    public void Prescribe_scales_sets_by_fitness_level_for_strength(TrainingStyle style, FitnessLevel level, int expectedSets)
    {
        var prescription = _sut.Prescribe(style, level);

        Assert.Equal(expectedSets, prescription.Sets);
    }

    [Theory]
    [InlineData(FitnessLevel.Beginner, 4)]
    [InlineData(FitnessLevel.Intermediate, 5)]
    [InlineData(FitnessLevel.Advanced, 6)]
    public void ExerciseCountForDay_increases_with_fitness_level(FitnessLevel level, int expectedCount)
    {
        Assert.Equal(expectedCount, _sut.ExerciseCountForDay(level));
    }
}
