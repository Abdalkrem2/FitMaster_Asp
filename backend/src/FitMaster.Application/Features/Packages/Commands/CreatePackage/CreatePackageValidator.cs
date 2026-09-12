using FluentValidation;

namespace FitMaster.Application.Features.Packages.Commands.CreatePackage;

public class CreatePackageValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DurationInDays).GreaterThan(0);
    }
}
