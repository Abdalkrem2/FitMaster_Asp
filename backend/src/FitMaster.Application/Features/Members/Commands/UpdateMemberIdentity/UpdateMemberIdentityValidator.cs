using FluentValidation;

namespace FitMaster.Application.Features.Members.Commands.UpdateMemberIdentity;

public class UpdateMemberIdentityValidator : AbstractValidator<UpdateMemberIdentityCommand>
{
    public UpdateMemberIdentityValidator()
    {
        RuleFor(x => x.MemberId).GreaterThan(0);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
    }
}
