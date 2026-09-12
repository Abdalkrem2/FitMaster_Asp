using FluentValidation;

namespace FitMaster.Application.Features.Nutrition.Commands.GenerateNutritionPlan;

public class GenerateNutritionPlanValidator : AbstractValidator<GenerateNutritionPlanCommand>
{
    public GenerateNutritionPlanValidator()
    {
        RuleFor(x => x.MemberId).GreaterThan(0);
    }
}
