using FluentValidation;

namespace FitMaster.Application.Features.Workouts.Commands.GenerateWorkoutPlan;

public class GenerateWorkoutPlanValidator : AbstractValidator<GenerateWorkoutPlanCommand>
{
    public GenerateWorkoutPlanValidator()
    {
        RuleFor(x => x.MemberId).GreaterThan(0);
    }
}
