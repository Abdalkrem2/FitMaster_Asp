using MediatR;

namespace FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlansByMember;

public record GetNutritionPlansByMemberQuery(long MemberId) : IRequest<List<NutritionPlanListItemDto>>;
