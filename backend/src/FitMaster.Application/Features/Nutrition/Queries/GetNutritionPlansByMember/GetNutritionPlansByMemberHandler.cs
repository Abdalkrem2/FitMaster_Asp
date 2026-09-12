using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlanById;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlansByMember;

public class GetNutritionPlansByMemberHandler(IApplicationDbContext db)
    : IRequestHandler<GetNutritionPlansByMemberQuery, List<NutritionPlanDto>>
{
    public Task<List<NutritionPlanDto>> Handle(GetNutritionPlansByMemberQuery request, CancellationToken cancellationToken)
        => db.NutritionPlans
            .Where(p => p.MemberId == request.MemberId)
            .OrderByDescending(p => p.CreatedAt)
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
            .ToListAsync(cancellationToken);
}
