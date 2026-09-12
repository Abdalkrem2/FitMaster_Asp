using FitMaster.Application.Common;
using FluentValidation;

namespace FitMaster.Application.Features.Exercises.Commands.UploadExerciseMedia;

public class UploadExerciseMediaValidator : AbstractValidator<UploadExerciseMediaCommand>
{
    public UploadExerciseMediaValidator()
    {
        RuleFor(x => x.ExerciseId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().WithMessage("File is empty.");
        RuleFor(x => x.Content.Length).LessThanOrEqualTo(ImageUploadRules.MaxSizeBytes)
            .WithMessage($"File exceeds the {ImageUploadRules.MaxSizeBytes / (1024 * 1024)}MB limit.");
        RuleFor(x => x.ContentType).Must(ImageUploadRules.AllowedContentTypes.Contains)
            .WithMessage("Unsupported file type. Allowed: " + string.Join(", ", ImageUploadRules.AllowedContentTypes));
    }
}
