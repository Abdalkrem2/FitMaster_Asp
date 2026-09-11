using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FitMaster.Application.Pdf;

public interface IWorkoutPlanPdfGenerator
{
    byte[] Generate(WorkoutPlanDto plan);
}

public class WorkoutPlanPdfGenerator : IWorkoutPlanPdfGenerator
{
    public byte[] Generate(WorkoutPlanDto plan)
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
                    col.Item().Text(plan.Name ?? "Workout Plan").FontSize(18).Bold();
                    col.Item().Text($"{plan.SplitType} - {plan.Goal} - {plan.Level}").FontSize(11);
                    col.Item().PaddingBottom(10).LineHorizontal(1);
                });

                page.Content().Column(col =>
                {
                    foreach (var day in plan.WorkoutDays)
                    {
                        col.Item().PaddingTop(10).Text($"Day {day.DayNumber}: {day.MuscleGroupLabel}").FontSize(13).Bold();

                        col.Item().PaddingTop(4).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Exercise").Bold();
                                header.Cell().Text("Sets").Bold();
                                header.Cell().Text("Reps").Bold();
                                header.Cell().ColumnSpan(3).PaddingBottom(2).LineHorizontal(0.5f);
                            });

                            foreach (var exercise in day.WorkoutExercises)
                            {
                                table.Cell().Text(exercise.ExerciseName ?? "Exercise");
                                table.Cell().Text(exercise.Sets?.ToString() ?? "-");
                                table.Cell().Text(FormatReps(exercise));
                            }
                        });
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

    private static string FormatReps(WorkoutExerciseDto exercise)
    {
        if (exercise.DurationSeconds is { } seconds) return $"{seconds}s";
        if (exercise.RepsMax is { } max && exercise.Reps is { } min && max != min) return $"{min}-{max}";
        return exercise.Reps?.ToString() ?? "-";
    }
}
