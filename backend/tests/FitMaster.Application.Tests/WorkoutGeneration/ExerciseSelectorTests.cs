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
    public void Blocks_a_named_advanced_movement_for_intermediate_and_advanced_members_too()
    {
        // The actual bug reported live: "full planche push-up" and "muscle up" showed
        // up in an Intermediate member's generated plan. The blocklist must apply
        // regardless of fitness level - these are specialist skill movements, not
        // something that becomes appropriate just because a member levels up.
        var pushDay = new DayTemplate("Push", [MuscleGroup.Chest]);
        var plancheCandidate = BenchPressCandidate with { Name = "full planche push-up", Difficulty = DifficultyLevel.Intermediate };

        var selectedForIntermediate = _sut.SelectForDay(
            pushDay, [plancheCandidate], FitnessLevel.Intermediate, new HashSet<long>(), [], exerciseCount: 1);
        var selectedForAdvanced = _sut.SelectForDay(
            pushDay, [plancheCandidate], FitnessLevel.Advanced, new HashSet<long>(), [], exerciseCount: 1);

        Assert.Empty(selectedForIntermediate);
        Assert.Empty(selectedForAdvanced);
    }

    [Theory]
    [InlineData("dumbbell one arm lateral raise")]
    [InlineData("cable one arm curl")]
    [InlineData("band one arm overhead biceps curl")]
    [InlineData("weighted pull-up")]
    [InlineData("kettlebell one arm push press")]
    public void Does_not_block_ordinary_unilateral_or_loaded_exercises(string exerciseName)
    {
        // Regression test for the previous "one arm"/"one-arm" keywords, which matched
        // 117 of 1,180 seeded strength exercises (~10% of the catalog) - almost all of
        // them completely normal unilateral dumbbell/cable/band work, not elite skills.
        var pushDay = new DayTemplate("Push", [MuscleGroup.Chest]);
        var candidate = BenchPressCandidate with { Name = exerciseName };

        var selected = _sut.SelectForDay(
            pushDay, [candidate], FitnessLevel.Beginner, new HashSet<long>(), [], exerciseCount: 1);

        Assert.Contains(BenchPress, selected);
    }

    [Theory]
    [InlineData("full maltese")]
    [InlineData("straddle maltese")]
    [InlineData("one arm chin-up")]
    [InlineData("one arm dip")]
    [InlineData("weighted one hand pull up")]
    [InlineData("kettlebell one arm jerk")]
    [InlineData("squat jerk")]
    public void Blocks_elite_skill_movements_found_in_the_seeded_catalog(string exerciseName)
    {
        // Found by querying the actual exercise_translations data for elite-skill name
        // patterns beyond the two originally-confirmed examples (planche, muscle-up).
        var pushDay = new DayTemplate("Push", [MuscleGroup.Chest]);
        var candidate = BenchPressCandidate with { Name = exerciseName };

        var selected = _sut.SelectForDay(
            pushDay, [candidate], FitnessLevel.Advanced, new HashSet<long>(), [], exerciseCount: 1);

        Assert.Empty(selected);
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

    [Fact]
    public void Upper_day_picks_more_chest_and_back_exercises_than_biceps_or_triceps()
    {
        // End-to-end: with an ample candidate pool for every Upper-day muscle group and
        // a big enough budget, the day-type weighting (chest/back=3, shoulders/triceps/
        // biceps=2) should actually show up in what SelectForDay picks - not just in the
        // allocator's output in isolation.
        const long Back = 20;
        const long Shoulders = 21;
        const long Triceps = 22;
        const long Biceps = 23;

        ExerciseCandidate Isolation(long muscleId, MuscleGroup group, int index) => new(
            Guid.NewGuid(), DifficultyLevel.Intermediate,
            [new MuscleTarget(muscleId, group, MuscleRole.Primary)],
            $"{group} isolation {index}");

        var candidates = new List<ExerciseCandidate>();
        foreach (var (muscleId, group) in new[]
        {
            (Chest, MuscleGroup.Chest), (Back, MuscleGroup.Back), (Shoulders, MuscleGroup.Shoulders),
            (Triceps, MuscleGroup.Triceps), (Biceps, MuscleGroup.Biceps),
        })
        {
            for (var i = 0; i < 4; i++) candidates.Add(Isolation(muscleId, group, i));
        }

        var upperDay = new DayTemplate(
            "Upper Body",
            [MuscleGroup.Chest, MuscleGroup.Back, MuscleGroup.Shoulders, MuscleGroup.Triceps, MuscleGroup.Biceps],
            new Dictionary<MuscleGroup, int>
            {
                [MuscleGroup.Chest] = 3,
                [MuscleGroup.Back] = 3,
                [MuscleGroup.Shoulders] = 2,
                [MuscleGroup.Triceps] = 2,
                [MuscleGroup.Biceps] = 2,
            });

        var selected = _sut.SelectForDay(
            upperDay, candidates, FitnessLevel.Advanced, new HashSet<long>(), [], exerciseCount: 12);

        int CountFor(MuscleGroup group) => selected.Count(id => candidates.Any(c =>
            c.ExerciseId == id && c.Targets.Any(t => t.Group == group)));

        Assert.Equal(12, selected.Count);
        Assert.Equal(3, CountFor(MuscleGroup.Chest));
        Assert.Equal(3, CountFor(MuscleGroup.Back));
        Assert.Equal(2, CountFor(MuscleGroup.Shoulders));
        Assert.Equal(2, CountFor(MuscleGroup.Triceps));
        Assert.Equal(2, CountFor(MuscleGroup.Biceps));
    }

    [Fact]
    public void Bodyweight_preference_excludes_any_exercise_that_requires_equipment()
    {
        var barbellBenchPress = BenchPressCandidate with { RequiresEquipment = true };
        var pushUp = LegExtensionCandidate with
        {
            ExerciseId = Guid.NewGuid(),
            Name = "push-up",
            Targets = [new MuscleTarget(Chest, MuscleGroup.Chest, MuscleRole.Primary)],
            RequiresEquipment = false,
        };
        var chestDay = new DayTemplate("Push", [MuscleGroup.Chest]);

        var selected = _sut.SelectForDay(
            chestDay, [barbellBenchPress, pushUp], FitnessLevel.Advanced, new HashSet<long>(), [], exerciseCount: 2,
            equipmentPreference: EquipmentPreference.Bodyweight);

        Assert.DoesNotContain(BenchPress, selected);
        Assert.Contains(pushUp.ExerciseId, selected);
    }

    [Fact]
    public void Gym_preference_is_unaffected_by_the_equipment_filter()
    {
        var barbellBenchPress = BenchPressCandidate with { RequiresEquipment = true };

        var selected = _sut.SelectForDay(
            new DayTemplate("Push", [MuscleGroup.Chest]), [barbellBenchPress], FitnessLevel.Advanced,
            new HashSet<long>(), [], exerciseCount: 1, equipmentPreference: EquipmentPreference.Gym);

        Assert.Contains(BenchPress, selected);
    }

    [Fact]
    public void Default_equipment_preference_behaves_as_gym_when_not_specified()
    {
        // No equipmentPreference argument at all - existing call sites (and the default
        // parameter value) should behave exactly like Gym, i.e. no filtering.
        var barbellBenchPress = BenchPressCandidate with { RequiresEquipment = true };

        var selected = _sut.SelectForDay(
            new DayTemplate("Push", [MuscleGroup.Chest]), [barbellBenchPress], FitnessLevel.Advanced,
            new HashSet<long>(), [], exerciseCount: 1);

        Assert.Contains(BenchPress, selected);
    }
}
