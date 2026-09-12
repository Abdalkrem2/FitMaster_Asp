using FluentValidation;

namespace FitMaster.Application.Features.Payments.Commands.CreatePaymentSession;

public class CreatePaymentSessionValidator : AbstractValidator<CreatePaymentSessionCommand>
{
    public CreatePaymentSessionValidator()
    {
        RuleFor(x => x.MemberId).GreaterThan(0);
        RuleFor(x => x.PackageId).GreaterThan(0);
        RuleFor(x => x.SuccessUrl).NotEmpty();
        RuleFor(x => x.CancelUrl).NotEmpty();
    }
}
