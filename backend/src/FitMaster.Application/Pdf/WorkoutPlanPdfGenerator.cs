using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FitMaster.Application.Pdf;

public interface IWorkoutPlanPdfGenerator
{
    Task<byte[]> GenerateAsync(WorkoutPlanDto plan, CancellationToken cancellationToken);
}

public class WorkoutPlanPdfGenerator(IExerciseImageFetcher imageFetcher) : IWorkoutPlanPdfGenerator
{
    public async Task<byte[]> GenerateAsync(WorkoutPlanDto plan, CancellationToken cancellationToken)
    {
        // QuestPDF's document-building callback below is synchronous, so every image
        // needed has to be fetched up front - deduplicated by URL since the same
        // exercise (and its image) can appear on more than one day.
        var imageUrls = plan.WorkoutDays
            .SelectMany(d => d.WorkoutExercises)
            .Select(e => e.ImageUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Distinct()
            .ToList();

        // Fetched in parallel - sequentially, ~30 small Cloudinary images added
        // 10-15s to plan download; independent requests, no reason to serialize them.
        var fetchResults = await Task.WhenAll(imageUrls.Select(async url => (Url: url!, Bytes: await imageFetcher.FetchAsync(url!, cancellationToken))));
        var images = fetchResults
            .Where(r => r.Bytes is not null)
            .ToDictionary(r => r.Url, r => r.Bytes!);

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
                });

                page.Content().Column(col =>
                {
                    foreach (var day in plan.WorkoutDays)
                    {
                        // Colored band, not just a bold line - makes each day
                        // unmistakably its own section when skimming the PDF.
                        col.Item().PaddingTop(14).Background(Colors.Indigo.Medium).Padding(8)
                            .Text($"Day {day.DayNumber} — {day.MuscleGroupLabel}")
                            .FontSize(13).Bold().FontColor(Colors.White);

                        var number = 1;
                        foreach (var exercise in day.WorkoutExercises)
                        {
                            byte[]? imageBytes = exercise.ImageUrl is not null && images.TryGetValue(exercise.ImageUrl, out var found)
                                ? found
                                : null;

                            col.Item().PaddingTop(8).Border(1).BorderColor(Colors.Grey.Lighten2)
                                .Padding(8).Row(row =>
                            {
                                row.ConstantItem(20).Text(number.ToString()).FontSize(11).Bold();

                                if (imageBytes is not null)
                                {
                                    row.ConstantItem(70).Height(70).Image(imageBytes).FitArea();
                                    row.ConstantItem(8);
                                }

                                row.RelativeItem().Column(details =>
                                {
                                    details.Item().Text(exercise.ExerciseName ?? "Exercise").Bold();
                                    for (var i = 0; i < exercise.Instructions.Count; i++)
                                    {
                                        details.Item().PaddingTop(1)
                                            .Text($"{i + 1}. {exercise.Instructions[i]}").FontSize(8.5f);
                                    }
                                });

                                row.ConstantItem(45).AlignRight().Text($"Sets\n{exercise.Sets?.ToString() ?? "-"}").FontSize(9);
                                row.ConstantItem(50).AlignRight().Text($"Reps\n{FormatReps(exercise)}").FontSize(9);
                            });

                            number++;
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

    private static string FormatReps(WorkoutExerciseDto exercise)
    {
        if (exercise.DurationSeconds is { } seconds) return $"{seconds}s";
        if (exercise.RepsMax is { } max && exercise.Reps is { } min && max != min) return $"{min}-{max}";
        return exercise.Reps?.ToString() ?? "-";
    }
}
