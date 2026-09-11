using System.Text;
using FitMaster.Domain.Entities.Members;

namespace FitMaster.Application.NutritionGeneration;

/// <summary>Builds the chat prompt sent to the AI meal-plan client. Pure/data-only - no I/O.</summary>
public interface INutritionPromptBuilder
{
    string Build(MacroTargets targets, MemberProfile profile);
}

public class NutritionPromptBuilder : INutritionPromptBuilder
{
    private const int MealCount = 5;

    public string Build(MacroTargets targets, MemberProfile profile)
    {
        var perMeal = targets.Calories / MealCount;
        var sb = new StringBuilder();

        sb.Append("Create a daily meal plan with EXACTLY ").Append(targets.Calories).Append(" kcal total.\n");
        sb.Append("Split across ").Append(MealCount).Append(" meals, each meal ~").Append(perMeal).Append(" kcal.\n\n");

        sb.Append("Goal: ").Append(profile.Goal).Append('\n');
        sb.Append("Targets: ").Append(targets.ProteinGrams).Append("g protein, ")
            .Append(targets.CarbsGrams).Append("g carbs, ")
            .Append(targets.FatGrams).Append("g fat\n");

        if (profile.HasDiabetes) sb.Append("Diabetic: low glycemic foods only\n");
        if (profile.HasHeartConditions) sb.Append("Heart condition: low saturated fat\n");
        if (profile.HasHypertension) sb.Append("Hypertension: low sodium\n");
        if (profile.Allergies.Count > 0)
        {
            sb.Append("AVOID: ").Append(string.Join(", ", profile.Allergies)).Append('\n');
        }

        sb.Append("\nRULES:\n");
        sb.Append("1. Return ONLY valid JSON, no markdown\n");
        sb.Append("2. Output MINIFIED JSON only - no indentation, no line breaks, no extra spaces\n");
        sb.Append("3. Exactly ").Append(MealCount).Append(" meals: Breakfast, Morning Snack, Lunch, Afternoon Snack, Dinner\n");
        sb.Append("4. Each meal must reach ~").Append(perMeal).Append(" kcal - use large portions\n");
        sb.Append("5. Exactly 2 foods per meal, short food names\n");
        sb.Append("6. Exactly 1 short recipe step per meal\n\n");

        sb.Append("JSON structure (minified, matching exactly):\n");
        sb.Append("{\"meals\":[{\"name\":\"Breakfast\",\"mealTime\":\"8:00 AM\",\"prepTime\":\"10 minutes\",");
        sb.Append("\"totalCalories\":").Append(perMeal).Append(',');
        sb.Append("\"foods\":[{\"name\":\"food\",\"amount\":\"100g\",\"calories\":300,");
        sb.Append("\"proteinGrams\":20,\"carbsGrams\":30,\"fatGrams\":10}],");
        sb.Append("\"recipeSteps\":[{\"stepOrder\":1,\"instruction\":\"step here\"}]}]}\n");

        return sb.ToString();
    }
}
