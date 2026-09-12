using FitMaster.Application.NutritionGeneration;
using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Enums;

namespace FitMaster.Application.Tests.NutritionGeneration;

public class CalorieCalculatorTests
{
    private readonly CalorieCalculator _sut = new();

    private static MemberProfile Profile(FitnessGoal goal, FitnessLevel level, double weight = 80, double height = 180, int age = 30) => new()
    {
        MemberId = 1,
        Goal = goal,
        FitnessLevel = level,
        SplitType = SplitType.FullBody,
        Weight = weight,
        Height = height,
        Age = age,
    };

    [Fact]
    public void Throws_when_weight_height_or_age_are_missing()
    {
        var profile = new MemberProfile
        {
            MemberId = 1,
            Goal = FitnessGoal.MuscleGain,
            FitnessLevel = FitnessLevel.Beginner,
            SplitType = SplitType.FullBody,
        };

        Assert.Throws<InvalidOperationException>(() => _sut.Calculate(profile, "Male"));
    }

    [Fact]
    public void MuscleGain_adds_a_calorie_surplus_over_general_fitness()
    {
        var muscleGain = _sut.Calculate(Profile(FitnessGoal.MuscleGain, FitnessLevel.Intermediate), "Male");
        var maintenance = _sut.Calculate(Profile(FitnessGoal.GeneralFitness, FitnessLevel.Intermediate), "Male");

        Assert.Equal(maintenance.Calories + 300, muscleGain.Calories);
    }

    [Fact]
    public void WeightLoss_creates_a_calorie_deficit_under_general_fitness()
    {
        var weightLoss = _sut.Calculate(Profile(FitnessGoal.WeightLoss, FitnessLevel.Intermediate), "Male");
        var maintenance = _sut.Calculate(Profile(FitnessGoal.GeneralFitness, FitnessLevel.Intermediate), "Male");

        Assert.Equal(maintenance.Calories - 500, weightLoss.Calories);
    }

    [Fact]
    public void Higher_fitness_level_increases_the_activity_multiplier_and_thus_calories()
    {
        var beginner = _sut.Calculate(Profile(FitnessGoal.GeneralFitness, FitnessLevel.Beginner), "Male");
        var advanced = _sut.Calculate(Profile(FitnessGoal.GeneralFitness, FitnessLevel.Advanced), "Male");

        Assert.True(advanced.Calories > beginner.Calories);
    }

    [Fact]
    public void Protein_target_is_two_grams_per_kilogram_of_bodyweight()
    {
        var targets = _sut.Calculate(Profile(FitnessGoal.GeneralFitness, FitnessLevel.Intermediate, weight: 80), "Male");

        Assert.Equal(160, targets.ProteinGrams);
    }

    [Fact]
    public void Macros_are_internally_consistent_with_the_calorie_target()
    {
        var targets = _sut.Calculate(Profile(FitnessGoal.MuscleGain, FitnessLevel.Advanced), "Female");

        var reconstructedCalories = (targets.ProteinGrams * 4) + (targets.CarbsGrams * 4) + (targets.FatGrams * 9);

        // Integer rounding at each step can drift it slightly - stay within a few kcal.
        Assert.InRange(reconstructedCalories, targets.Calories - 5, targets.Calories + 5);
    }

    [Fact]
    public void Unspecified_gender_uses_the_same_formula_as_female()
    {
        var female = _sut.Calculate(Profile(FitnessGoal.GeneralFitness, FitnessLevel.Intermediate), "Female");
        var unspecified = _sut.Calculate(Profile(FitnessGoal.GeneralFitness, FitnessLevel.Intermediate), null);

        Assert.Equal(female.Calories, unspecified.Calories);
    }
}
