using FitMaster.Application.WorkoutGeneration;

namespace FitMaster.Application.Tests.WorkoutGeneration;

public class MuscleGroupAllocatorTests
{
    [Fact]
    public void Gives_one_of_each_high_priority_group_before_any_group_gets_a_second_slot()
    {
        // Upper-day-shaped weights (chest/back the biggest movers), but a tight budget
        // (5 slots for 5 groups) - every group should get exactly one exercise, not
        // "3 chest and nothing for triceps/biceps" just because chest has the highest weight.
        var groups = new[] { MuscleGroup.Chest, MuscleGroup.Back, MuscleGroup.Shoulders, MuscleGroup.Triceps, MuscleGroup.Biceps };
        var weights = new Dictionary<MuscleGroup, int>
        {
            [MuscleGroup.Chest] = 3,
            [MuscleGroup.Back] = 3,
            [MuscleGroup.Shoulders] = 2,
            [MuscleGroup.Triceps] = 2,
            [MuscleGroup.Biceps] = 2,
        };

        var allocation = MuscleGroupAllocator.Allocate(groups, weights, totalSlots: 5);

        Assert.Equal(groups.Length, allocation.Count);
        foreach (var group in groups)
        {
            Assert.Equal(1, allocation.Count(g => g == group));
        }
    }

    [Fact]
    public void Gives_higher_weighted_groups_more_slots_once_the_budget_allows_it()
    {
        var groups = new[] { MuscleGroup.Chest, MuscleGroup.Back, MuscleGroup.Shoulders, MuscleGroup.Triceps, MuscleGroup.Biceps };
        var weights = new Dictionary<MuscleGroup, int>
        {
            [MuscleGroup.Chest] = 3,
            [MuscleGroup.Back] = 3,
            [MuscleGroup.Shoulders] = 2,
            [MuscleGroup.Triceps] = 2,
            [MuscleGroup.Biceps] = 2,
        };

        // Full "generous" budget (matches the sum of weights exactly).
        var allocation = MuscleGroupAllocator.Allocate(groups, weights, totalSlots: 12);

        Assert.Equal(3, allocation.Count(g => g == MuscleGroup.Chest));
        Assert.Equal(3, allocation.Count(g => g == MuscleGroup.Back));
        Assert.Equal(2, allocation.Count(g => g == MuscleGroup.Shoulders));
        Assert.Equal(2, allocation.Count(g => g == MuscleGroup.Triceps));
        Assert.Equal(2, allocation.Count(g => g == MuscleGroup.Biceps));
    }

    [Fact]
    public void Drops_the_lowest_priority_groups_first_when_the_budget_is_smaller_than_the_group_count()
    {
        // Full Body day: 7 groups, ordered by priority, but a Beginner's small budget
        // (4 slots) can't cover all of them - the highest-priority groups (first in the
        // list) should win, not an arbitrary subset.
        var groups = new[]
        {
            MuscleGroup.Quadriceps, MuscleGroup.Back, MuscleGroup.Chest, MuscleGroup.Hamstrings,
            MuscleGroup.Shoulders, MuscleGroup.Glutes, MuscleGroup.Abs,
        };
        var weights = groups.ToDictionary(g => g, _ => 1);

        var allocation = MuscleGroupAllocator.Allocate(groups, weights, totalSlots: 4);

        Assert.Equal(
            new[] { MuscleGroup.Quadriceps, MuscleGroup.Back, MuscleGroup.Chest, MuscleGroup.Hamstrings },
            allocation);
    }

    [Fact]
    public void Falls_back_to_plain_round_robin_once_every_weight_is_exhausted()
    {
        // Budget bigger than the sum of weights (2+1=3) - the extra slots must still
        // get spent instead of leaving the day under-filled.
        var groups = new[] { MuscleGroup.Chest, MuscleGroup.Back };
        var weights = new Dictionary<MuscleGroup, int> { [MuscleGroup.Chest] = 2, [MuscleGroup.Back] = 1 };

        var allocation = MuscleGroupAllocator.Allocate(groups, weights, totalSlots: 6);

        Assert.Equal(6, allocation.Count);
    }

    [Fact]
    public void Treats_a_missing_weight_as_one_so_no_weights_at_all_behaves_as_plain_round_robin()
    {
        var groups = new[] { MuscleGroup.Chest, MuscleGroup.Back, MuscleGroup.Shoulders };

        var allocation = MuscleGroupAllocator.Allocate(groups, weights: null, totalSlots: 3);

        Assert.Equal(groups, allocation);
    }
}
