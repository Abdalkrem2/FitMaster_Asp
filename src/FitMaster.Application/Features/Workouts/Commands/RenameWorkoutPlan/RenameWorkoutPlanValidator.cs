using FluentValidation;

namespace FitMaster.Application.Features.Workouts.Commands.RenameWorkoutPlan;

public class RenameWorkoutPlanValidator : AbstractValidator<RenameWorkoutPlanCommand>
{
    public RenameWorkoutPlanValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
