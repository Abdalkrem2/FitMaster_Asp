using FluentValidation;

namespace FitMaster.Application.Features.Members.Commands.UpdateMemberProfile;

public class UpdateMemberProfileValidator : AbstractValidator<UpdateMemberProfileCommand>
{
    public UpdateMemberProfileValidator()
    {
        RuleFor(x => x.Weight).GreaterThan(0).When(x => x.Weight.HasValue);
        RuleFor(x => x.Height).GreaterThan(0).When(x => x.Height.HasValue);
        RuleFor(x => x.Age).InclusiveBetween(1, 120).When(x => x.Age.HasValue);
    }
}
