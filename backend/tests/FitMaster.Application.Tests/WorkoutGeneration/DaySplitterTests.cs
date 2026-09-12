using FitMaster.Application.WorkoutGeneration;
using FitMaster.Domain.Enums;

namespace FitMaster.Application.Tests.WorkoutGeneration;

public class DaySplitterTests
{
    private readonly DaySplitter _sut = new();

    [Fact]
    public void FullBody_produces_three_days_each_hitting_every_major_group()
    {
        var days = _sut.Split(SplitType.FullBody);

        Assert.Equal(3, days.Count);
        Assert.All(days, d => Assert.Equal("Full Body", d.Label));
        Assert.All(days, d =>
        {
            Assert.Contains(MuscleGroup.Chest, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Back, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Quadriceps, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Hamstrings, d.TargetMuscleGroups);
        });
    }

    [Fact]
    public void UpperLower_produces_four_days_alternating_upper_and_lower()
    {
        var days = _sut.Split(SplitType.UpperLower);

        Assert.Equal(4, days.Count);
        Assert.Equal(["Upper Body", "Lower Body", "Upper Body", "Lower Body"], days.Select(d => d.Label));

        // Upper days never target leg groups, lower days never target upper-body pushing groups.
        Assert.All(days.Where(d => d.Label == "Upper Body"), d =>
        {
            Assert.DoesNotContain(MuscleGroup.Quadriceps, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Chest, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Back, d.TargetMuscleGroups);
        });
        Assert.All(days.Where(d => d.Label == "Lower Body"), d =>
        {
            Assert.DoesNotContain(MuscleGroup.Chest, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Quadriceps, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Glutes, d.TargetMuscleGroups);
        });
    }

    [Fact]
    public void PushPullLegs_produces_six_days_each_group_hit_twice()
    {
        var days = _sut.Split(SplitType.PushPullLegs);

        Assert.Equal(6, days.Count);
        Assert.Equal(["Push", "Pull", "Legs", "Push", "Pull", "Legs"], days.Select(d => d.Label));

        var pushDays = days.Where(d => d.Label == "Push").ToList();
        Assert.All(pushDays, d =>
        {
            Assert.Contains(MuscleGroup.Chest, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Triceps, d.TargetMuscleGroups);
            Assert.DoesNotContain(MuscleGroup.Back, d.TargetMuscleGroups);
        });

        var pullDays = days.Where(d => d.Label == "Pull").ToList();
        Assert.All(pullDays, d =>
        {
            Assert.Contains(MuscleGroup.Back, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Biceps, d.TargetMuscleGroups);
            Assert.DoesNotContain(MuscleGroup.Chest, d.TargetMuscleGroups);
        });

        var legDays = days.Where(d => d.Label == "Legs").ToList();
        Assert.All(legDays, d =>
        {
            Assert.Contains(MuscleGroup.Quadriceps, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Hamstrings, d.TargetMuscleGroups);
            Assert.Contains(MuscleGroup.Glutes, d.TargetMuscleGroups);
        });
    }

    [Fact]
    public void Upper_day_weights_chest_and_back_higher_than_arms()
    {
        var upperDay = _sut.Split(SplitType.UpperLower).First(d => d.Label == "Upper Body");

        Assert.NotNull(upperDay.MuscleGroupWeights);
        var weights = upperDay.MuscleGroupWeights!;

        Assert.True(weights[MuscleGroup.Chest] > weights[MuscleGroup.Biceps]);
        Assert.True(weights[MuscleGroup.Back] > weights[MuscleGroup.Triceps]);
    }

    [Fact]
    public void Pull_day_weights_back_higher_than_biceps()
    {
        var pullDay = _sut.Split(SplitType.PushPullLegs).First(d => d.Label == "Pull");

        Assert.NotNull(pullDay.MuscleGroupWeights);
        Assert.True(pullDay.MuscleGroupWeights![MuscleGroup.Back] > pullDay.MuscleGroupWeights[MuscleGroup.Biceps]);
    }

    [Fact]
    public void Legs_day_weights_quadriceps_at_least_as_high_as_calves()
    {
        var legsDay = _sut.Split(SplitType.PushPullLegs).First(d => d.Label == "Legs");

        Assert.NotNull(legsDay.MuscleGroupWeights);
        Assert.True(legsDay.MuscleGroupWeights![MuscleGroup.Quadriceps] >= legsDay.MuscleGroupWeights[MuscleGroup.Calves]);
    }

    [Fact]
    public void Every_split_types_day_templates_carry_muscle_group_weights()
    {
        foreach (var splitType in new[] { SplitType.FullBody, SplitType.UpperLower, SplitType.PushPullLegs })
        {
            var days = _sut.Split(splitType);
            Assert.All(days, d => Assert.NotNull(d.MuscleGroupWeights));
        }
    }
}
