using FluentValidation;

namespace FitMaster.Application.Features.Members.Commands.CreateMember;

public class CreateMemberValidator : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Weight).GreaterThan(0).When(x => x.Weight.HasValue);
        RuleFor(x => x.Height).GreaterThan(0).When(x => x.Height.HasValue);
        RuleFor(x => x.Age).InclusiveBetween(1, 120).When(x => x.Age.HasValue);
    }
}
