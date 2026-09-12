using FitMaster.Domain.Enums;
using FluentValidation;

namespace FitMaster.Application.Features.Users.Commands.UpdateStaffUser;

public class UpdateStaffUserValidator : AbstractValidator<UpdateStaffUserCommand>
{
    public UpdateStaffUserValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Password).MinimumLength(8).When(x => x.Password is not null);
        RuleFor(x => x.Role)
            .Must(role => role is AppRole.Admin or AppRole.Employee)
            .When(x => x.Role is not null)
            .WithMessage("Role must be Admin or Employee.");
    }
}
