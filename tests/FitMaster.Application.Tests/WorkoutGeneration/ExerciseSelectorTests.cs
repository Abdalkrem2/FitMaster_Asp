using FitMaster.Application.WorkoutGeneration;
using FitMaster.Domain.Enums;

namespace FitMaster.Application.Tests.WorkoutGeneration;

public class ExerciseSelectorTests
{
    private readonly ExerciseSelector _sut = new();

    private const long Quadriceps = 11;
    private const long Glutes = 14;
    private const long Hamstrings = 13;
    private const long Chest = 15;

    private static readonly Guid Squat = Guid.NewGuid();
    private static readonly Guid LegExtension = Guid.NewGuid();
    private static readonly Guid BenchPress = Guid.NewGuid();

    // A compound leg exercise: hits 4 distinct muscles, quadriceps as its primary mover.
    private static readonly ExerciseCandidate SquatCandidate = new(
        Squat,
        DifficultyLevel.Intermediate,
        [
            new MuscleTarget(Quadriceps, MuscleGroup.Quadriceps, MuscleRole.Primary),
            new MuscleTarget(Glutes, MuscleGroup.Glutes, MuscleRole.Secondary),
            new MuscleTarget(Hamstrings, MuscleGroup.Hamstrings, MuscleRole.Secondary),
            new MuscleTarget(Chest, MuscleGroup.Chest, MuscleRole.Secondary),
        ]);

    // An isolation leg exercise: hits only quadriceps.
    private static readonly ExerciseCandidate LegExtensionCandidate = new(
        LegExtension,
        DifficultyLevel.Beginner,
        [new MuscleTarget(Quadriceps, MuscleGroup.Quadriceps, MuscleRole.Primary)]);

    private static readonly ExerciseCandidate BenchPressCandidate = new(
        BenchPress,
        DifficultyLevel.Beginner,
        [
            new MuscleTarget(Chest, MuscleGroup.Chest, MuscleRole.Primary),
            new MuscleTarget(Glutes, MuscleGroup.Glutes, MuscleRole.Secondary), // arbitrary, just to reach compound threshold
            new MuscleTarget(Hamstrings, MuscleGroup.Hamstrings, MuscleRole.Secondary),
            new MuscleTarget(Quadriceps, MuscleGroup.Quadriceps, MuscleRole.Secondary),
        ]);

    private static readonly DayTemplate LegDay = new("Legs", [MuscleGroup.Quadriceps]);

    [Fact]
    public void Prefers_the_compound_movement_over_isolation_for_the_same_muscle_group()
    {
        var candidates = new List<ExerciseCandidate> { LegExtensionCandidate, SquatCandidate };

        var selected = _sut.SelectForDay(
            LegDay, candidates, FitnessLevel.Advanced, new HashSet<long>(), [], exerciseCount: 1);

        Assert.Equal(Squat, Assert.Single(selected));
    }

    [Fact]
    public void Knee_injury_excludes_an_exercise_whose_primary_target_is_quadriceps()
    {
        // "Beginner, knee injury -> generated plan contains no squat-pattern exercise."
        var candidates = new List<ExerciseCandidate> { SquatCandidate, LegExtensionCandidate };
        var kneeExcludedMuscles = new HashSet<long> { Quadriceps, Glutes, Hamstrings };

        var selected = _sut.SelectForDay(
            LegDay, candidates, FitnessLevel.Beginner, kneeExcludedMuscles, [], exerciseCount: 4);

        Assert.DoesNotContain(Squat, selected);
        Assert.DoesNotContain(LegExtension, selected);
    }

    [Fact]
    public void Deprioritizes_but_does_not_exclude_an_exercise_whose_excluded_muscle_is_only_secondary()
    {
        // Bench press only touches the excluded muscle (quadriceps) as a secondary mover,
        // so it should still be selectable - just ranked behind an unaffected alternative.
        var chestDay = new DayTemplate("Push", [MuscleGroup.Chest]);
        var kneeExcludedMuscles = new HashSet<long> { Quadriceps };

        var selected = _sut.SelectForDay(
            chestDay, [BenchPressCandidate], FitnessLevel.Beginner, kneeExcludedMuscles, [], exerciseCount: 1);

        Assert.Contains(BenchPress, selected);
    }

    [Fact]
    public void Excludes_exercises_more_than_one_tier_above_the_members_fitness_level()
    {
        var advancedOnly = SquatCandidate with { Difficulty = DifficultyLevel.Advanced };

        var selected = _sut.SelectForDay(
            LegDay, [advancedOnly], FitnessLevel.Beginner, new HashSet<long>(), [], exerciseCount: 1);

        Assert.Empty(selected);
    }

    [Fact]
    public void Allows_a_beginner_one_tier_above_their_level_since_the_seeded_catalog_is_uniformly_intermediate()
    {
        // Every seeded exercise is currently classified Intermediate - without this
        // leniency, Beginner members would get zero eligible exercises, ever.
        var intermediateOnly = SquatCandidate with { Difficulty = DifficultyLevel.Intermediate };

        var selected = _sut.SelectForDay(
            LegDay, [intermediateOnly], FitnessLevel.Beginner, new HashSet<long>(), [], exerciseCount: 1);

        Assert.Contains(Squat, selected);
    }

    [Fact]
    public void Blocks_a_named_advanced_movement_for_beginners_even_when_tagged_intermediate()
    {
        var pushDay = new DayTemplate("Push", [MuscleGroup.Chest]);
        var plancheCandidate = BenchPressCandidate with { Name = "full planche push-up", Difficulty = DifficultyLevel.Intermediate };

        var selected = _sut.SelectForDay(
            pushDay, [plancheCandidate], FitnessLevel.Beginner, new HashSet<long>(), [], exerciseCount: 1);

        Assert.Empty(selected);
    }

    [Fact]
    public void Does_not_block_a_named_advanced_movement_for_non_beginners()
    {
        var pushDay = new DayTemplate("Push", [MuscleGroup.Chest]);
        var plancheCandidate = BenchPressCandidate with { Name = "full planche push-up", Difficulty = DifficultyLevel.Intermediate };

        var selected = _sut.SelectForDay(
            pushDay, [plancheCandidate], FitnessLevel.Intermediate, new HashSet<long>(), [], exerciseCount: 1);

        Assert.Contains(BenchPress, selected);
    }

    [Fact]
    public void Does_not_reselect_an_exercise_already_used_earlier_in_the_plan()
    {
        var usedExerciseIds = new HashSet<Guid> { Squat };

        var selected = _sut.SelectForDay(
            LegDay, [SquatCandidate, LegExtensionCandidate], FitnessLevel.Advanced, new HashSet<long>(), usedExerciseIds, exerciseCount: 4);

        Assert.DoesNotContain(Squat, selected);
        Assert.Contains(LegExtension, selected);
    }

    [Fact]
    public void Never_selects_more_than_the_requested_exercise_count()
    {
        var candidates = new List<ExerciseCandidate> { SquatCandidate, LegExtensionCandidate, BenchPressCandidate };

        var selected = _sut.SelectForDay(
            LegDay, candidates, FitnessLevel.Advanced, new HashSet<long>(), [], exerciseCount: 2);

        Assert.True(selected.Count <= 2);
    }
}
