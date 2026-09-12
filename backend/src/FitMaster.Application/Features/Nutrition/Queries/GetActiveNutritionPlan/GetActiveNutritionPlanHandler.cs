using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlanById;
using FitMaster.Domain.Entities.Nutrition;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Nutrition.Queries.GetActiveNutritionPlan;

public class GetActiveNutritionPlanHandler(IApplicationDbContext db) : IRequestHandler<GetActiveNutritionPlanQuery, NutritionPlanDto>
{
    public async Task<NutritionPlanDto> Handle(GetActiveNutritionPlanQuery request, CancellationToken cancellationToken)
    {
        var plan = await db.NutritionPlans
            .Where(p => p.MemberId == request.MemberId && p.Status == WorkoutPlanStatus.Active)
            .Select(p => new NutritionPlanDto(
                p.Id,
                p.MemberId,
                p.Goal,
                p.DailyCalories,
                p.ProteinGrams,
                p.CarbsGrams,
                p.FatGrams,
                p.Status,
                p.CreatedAt,
                p.Meals
                    .Select(m => new NutritionMealDto(
                        m.Id,
                        m.Name,
                        m.MealTime,
                        m.PrepTime,
                        m.TotalCalories,
                        m.Foods
                            .Select(f => new NutritionFoodDto(f.Id, f.Name, f.Amount, f.Calories, f.ProteinGrams, f.CarbsGrams, f.FatGrams))
                            .ToList(),
                        m.RecipeSteps
                            .OrderBy(s => s.StepOrder)
                            .Select(s => new NutritionRecipeStepDto(s.Id, s.StepOrder, s.Instruction))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        return plan ?? throw new NotFoundException(nameof(NutritionPlan), request.MemberId);
    }
}
