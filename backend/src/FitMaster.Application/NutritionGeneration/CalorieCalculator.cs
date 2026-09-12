using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Enums;

namespace FitMaster.Application.NutritionGeneration;

public record MacroTargets(int Calories, int ProteinGrams, int CarbsGrams, int FatGrams);

/// <summary>
/// Computes daily calorie/macro targets from a member's profile. Pure and
/// deterministic - ported from the original Java algorithm's Mifflin-St Jeor
/// calculation, which (unlike the workout generator) was sound and worth keeping.
/// </summary>
public interface ICalorieCalculator
{
    MacroTargets Calculate(MemberProfile profile, string? gender);
}

public class CalorieCalculator : ICalorieCalculator
{
    public MacroTargets Calculate(MemberProfile profile, string? gender)
    {
        if (profile.Weight is not { } weight || profile.Height is not { } height || profile.Age is not { } age)
        {
            throw new InvalidOperationException(
                "Weight, height and age must all be set on the member's profile to calculate nutrition targets.");
        }

        // Mifflin-St Jeor. Gender is free text and only ever recorded as "Male" in
        // practice, so anything else (including unset) uses the female formula -
        // same binary treatment the original algorithm used.
        var bmr = string.Equals(gender, "Male", StringComparison.OrdinalIgnoreCase)
            ? (10 * weight) + (6.25 * height) - (5 * age) + 5
            : (10 * weight) + (6.25 * height) - (5 * age) - 161;

        var tdee = profile.FitnessLevel switch
        {
            FitnessLevel.Beginner => bmr * 1.375,
            FitnessLevel.Intermediate => bmr * 1.55,
            FitnessLevel.Advanced => bmr * 1.725,
            _ => bmr * 1.2,
        };

        var calories = (int)(profile.Goal switch
        {
            FitnessGoal.MuscleGain => tdee + 300,
            FitnessGoal.WeightLoss => tdee - 500,
            FitnessGoal.Endurance => tdee + 200,
            FitnessGoal.GeneralFitness => tdee,
            _ => tdee,
        });

        var protein = (int)(weight * 2);
        var fat = (int)((calories * 0.25) / 9);
        var carbs = (int)((calories - (protein * 4) - (fat * 9)) / 4);

        return new MacroTargets(calories, protein, carbs, fat);
    }
}
