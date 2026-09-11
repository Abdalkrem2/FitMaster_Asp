using FitMaster.Domain.Enums;
using FluentValidation;

namespace FitMaster.Application.Features.Users.Commands.CreateStaffUser;

public class CreateStaffUserValidator : AbstractValidator<CreateStaffUserCommand>
{
    public CreateStaffUserValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Role)
            .Must(role => role is AppRole.Admin or AppRole.Employee)
            .WithMessage("Role must be Admin or Employee. Use the Members endpoint to add a Member.");
    }
}
