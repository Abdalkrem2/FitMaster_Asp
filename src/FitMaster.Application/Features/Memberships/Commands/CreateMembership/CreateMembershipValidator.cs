using FluentValidation;

namespace FitMaster.Application.Features.Memberships.Commands.CreateMembership;

public class CreateMembershipValidator : AbstractValidator<CreateMembershipCommand>
{
    public CreateMembershipValidator()
    {
        RuleFor(x => x.MemberId).GreaterThan(0);
        RuleFor(x => x.PackageId).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
        RuleFor(x => x.Debt).GreaterThanOrEqualTo(0).When(x => x.Debt.HasValue);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
