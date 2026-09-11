using FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlanById;
using MediatR;

namespace FitMaster.Application.Features.Nutrition.Queries.GetActiveNutritionPlan;

public record GetActiveNutritionPlanQuery(long MemberId) : IRequest<NutritionPlanDto>;
