using MediatR;

namespace FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlanById;

public record GetNutritionPlanByIdQuery(long Id) : IRequest<NutritionPlanDto>;
