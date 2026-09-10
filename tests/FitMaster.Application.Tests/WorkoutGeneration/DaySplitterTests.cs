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
}
