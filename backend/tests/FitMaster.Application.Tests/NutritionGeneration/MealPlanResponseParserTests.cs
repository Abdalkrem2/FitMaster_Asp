using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.NutritionGeneration;

namespace FitMaster.Application.Tests.NutritionGeneration;

public class MealPlanResponseParserTests
{
    private readonly MealPlanResponseParser _sut = new();

    private const string ValidJson = """
        {"meals":[
            {"name":"Breakfast","mealTime":"8:00 AM","prepTime":"10 minutes","totalCalories":500,
             "foods":[{"name":"Oatmeal","amount":"100g","calories":300,"proteinGrams":10,"carbsGrams":50,"fatGrams":5},
                      {"name":"Banana","amount":"1 medium","calories":200,"proteinGrams":2,"carbsGrams":45,"fatGrams":1}],
             "recipeSteps":[{"stepOrder":1,"instruction":"Cook oats."}]},
            {"name":"Lunch","mealTime":"1:00 PM","prepTime":null,"totalCalories":600,
             "foods":[{"name":"Chicken","amount":"200g","calories":400,"proteinGrams":40,"carbsGrams":0,"fatGrams":10},
                      {"name":"Rice","amount":"150g","calories":200,"proteinGrams":4,"carbsGrams":45,"fatGrams":1}],
             "recipeSteps":[]},
            {"name":"Dinner","mealTime":"7:00 PM","prepTime":"20 minutes","totalCalories":550,
             "foods":[{"name":"Salmon","amount":"180g","calories":400,"proteinGrams":35,"carbsGrams":0,"fatGrams":20},
                      {"name":"Broccoli","amount":"150g","calories":150,"proteinGrams":5,"carbsGrams":20,"fatGrams":2}],
             "recipeSteps":[{"stepOrder":1,"instruction":"Bake salmon."},{"stepOrder":2,"instruction":"Steam broccoli."}]}
        ]}
        """;

    [Fact]
    public void Parses_a_well_formed_response()
    {
        var result = _sut.Parse(ValidJson);

        Assert.Equal(3, result.Meals.Count);
        Assert.Equal("Breakfast", result.Meals[0].Name);
        Assert.Equal(2, result.Meals[0].Foods.Count);
        Assert.Single(result.Meals[0].RecipeSteps);
        Assert.Empty(result.Meals[1].RecipeSteps);
    }

    [Fact]
    public void Strips_markdown_code_fences_around_the_json()
    {
        var fenced = $"```json\n{ValidJson}\n```";

        var result = _sut.Parse(fenced);

        Assert.Equal(3, result.Meals.Count);
    }

    [Fact]
    public void Throws_on_malformed_json()
    {
        Assert.Throws<InvalidAiResponseException>(() => _sut.Parse("not json at all"));
    }

    [Fact]
    public void Throws_when_fewer_than_three_meals_are_returned()
    {
        const string twoMeals = """
            {"meals":[
                {"name":"Breakfast","mealTime":"8:00 AM","foods":[{"name":"Oats","amount":"100g","calories":300,"proteinGrams":10,"carbsGrams":50,"fatGrams":5}]},
                {"name":"Lunch","mealTime":"1:00 PM","foods":[{"name":"Chicken","amount":"200g","calories":400,"proteinGrams":40,"carbsGrams":0,"fatGrams":10}]}
            ]}
            """;

        Assert.Throws<InvalidAiResponseException>(() => _sut.Parse(twoMeals));
    }

    [Fact]
    public void Throws_when_a_meal_has_no_foods()
    {
        const string emptyFoods = """
            {"meals":[
                {"name":"Breakfast","mealTime":"8:00 AM","foods":[]},
                {"name":"Lunch","mealTime":"1:00 PM","foods":[{"name":"Chicken","amount":"200g","calories":400,"proteinGrams":40,"carbsGrams":0,"fatGrams":10}]},
                {"name":"Dinner","mealTime":"7:00 PM","foods":[{"name":"Salmon","amount":"180g","calories":400,"proteinGrams":35,"carbsGrams":0,"fatGrams":20}]}
            ]}
            """;

        Assert.Throws<InvalidAiResponseException>(() => _sut.Parse(emptyFoods));
    }
}
