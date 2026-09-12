using FitMaster.Application.Common;
using FluentValidation;

namespace FitMaster.Application.Features.Members.Commands.UploadProfilePicture;

public class UploadProfilePictureValidator : AbstractValidator<UploadProfilePictureCommand>
{
    public UploadProfilePictureValidator()
    {
        RuleFor(x => x.MemberId).GreaterThan(0);
        RuleFor(x => x.Content).NotEmpty().WithMessage("File is empty.");
        RuleFor(x => x.Content.Length).LessThanOrEqualTo(ImageUploadRules.MaxSizeBytes)
            .WithMessage($"File exceeds the {ImageUploadRules.MaxSizeBytes / (1024 * 1024)}MB limit.");
        RuleFor(x => x.ContentType).Must(ImageUploadRules.AllowedContentTypes.Contains)
            .WithMessage("Unsupported file type. Allowed: " + string.Join(", ", ImageUploadRules.AllowedContentTypes));
    }
}
