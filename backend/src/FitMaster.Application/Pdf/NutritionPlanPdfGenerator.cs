using FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlanById;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FitMaster.Application.Pdf;

public interface INutritionPlanPdfGenerator
{
    byte[] Generate(NutritionPlanDto plan);
}

public class NutritionPlanPdfGenerator : INutritionPlanPdfGenerator
{
    public byte[] Generate(NutritionPlanDto plan)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("Nutrition Plan").FontSize(18).Bold();
                    col.Item().Text($"Goal: {plan.Goal}").FontSize(11);
                    col.Item().Text(
                        $"{plan.DailyCalories} kcal - {plan.ProteinGrams}g protein, {plan.CarbsGrams}g carbs, {plan.FatGrams}g fat");
                    col.Item().PaddingBottom(10).LineHorizontal(1);
                });

                page.Content().Column(col =>
                {
                    foreach (var meal in plan.Meals)
                    {
                        col.Item().PaddingTop(10).Text($"{meal.Name} ({meal.MealTime}) - {meal.TotalCalories} kcal").FontSize(13).Bold();

                        col.Item().PaddingTop(4).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Food").Bold();
                                header.Cell().Text("Amount").Bold();
                                header.Cell().Text("Kcal").Bold();
                                header.Cell().Text("P").Bold();
                                header.Cell().Text("C").Bold();
                                header.Cell().Text("F").Bold();
                                header.Cell().ColumnSpan(6).PaddingBottom(2).LineHorizontal(0.5f);
                            });

                            foreach (var food in meal.Foods)
                            {
                                table.Cell().Text(food.Name);
                                table.Cell().Text(food.Amount);
                                table.Cell().Text(food.Calories.ToString());
                                table.Cell().Text($"{food.ProteinGrams}g");
                                table.Cell().Text($"{food.CarbsGrams}g");
                                table.Cell().Text($"{food.FatGrams}g");
                            }
                        });

                        if (meal.RecipeSteps.Count > 0)
                        {
                            col.Item().PaddingTop(4).Column(steps =>
                            {
                                foreach (var step in meal.RecipeSteps.OrderBy(s => s.StepOrder))
                                {
                                    steps.Item().Text($"{step.StepOrder}. {step.Instruction}").FontSize(9);
                                }
                            });
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }
}
