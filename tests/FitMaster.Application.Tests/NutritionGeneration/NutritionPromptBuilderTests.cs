using FitMaster.Application.NutritionGeneration;
using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Enums;

namespace FitMaster.Application.Tests.NutritionGeneration;

public class NutritionPromptBuilderTests
{
    private readonly NutritionPromptBuilder _sut = new();

    private static MemberProfile Profile(bool diabetes = false, bool heart = false, bool hypertension = false, List<AllergyType>? allergies = null) => new()
    {
        MemberId = 1,
        Goal = FitnessGoal.WeightLoss,
        FitnessLevel = FitnessLevel.Intermediate,
        SplitType = SplitType.FullBody,
        HasDiabetes = diabetes,
        HasHeartConditions = heart,
        HasHypertension = hypertension,
        Allergies = allergies ?? [],
    };

    [Fact]
    public void Includes_the_calorie_and_macro_targets()
    {
        var prompt = _sut.Build(new MacroTargets(2000, 150, 200, 60), Profile());

        Assert.Contains("2000 kcal", prompt);
        Assert.Contains("150g protein", prompt);
    }

    [Fact]
    public void Mentions_health_conditions_when_flagged()
    {
        var prompt = _sut.Build(new MacroTargets(2000, 150, 200, 60), Profile(diabetes: true, heart: true, hypertension: true));

        Assert.Contains("Diabetic", prompt);
        Assert.Contains("Heart condition", prompt);
        Assert.Contains("Hypertension", prompt);
    }

    [Fact]
    public void Omits_health_condition_lines_when_not_flagged()
    {
        var prompt = _sut.Build(new MacroTargets(2000, 150, 200, 60), Profile());

        Assert.DoesNotContain("Diabetic", prompt);
        Assert.DoesNotContain("Heart condition", prompt);
        Assert.DoesNotContain("Hypertension", prompt);
    }

    [Fact]
    public void Lists_allergies_to_avoid()
    {
        var prompt = _sut.Build(new MacroTargets(2000, 150, 200, 60), Profile(allergies: [AllergyType.Nuts, AllergyType.Shellfish]));

        Assert.Contains("AVOID", prompt);
        Assert.Contains("Nuts", prompt);
        Assert.Contains("Shellfish", prompt);
    }
}
