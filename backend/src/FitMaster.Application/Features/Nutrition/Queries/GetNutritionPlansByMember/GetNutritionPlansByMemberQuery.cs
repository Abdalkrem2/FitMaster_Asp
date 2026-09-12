using FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlanById;
using MediatR;

namespace FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlansByMember;

/// <summary>Full plan detail per entry (not a lightweight summary) - see the
/// matching workout query for why.</summary>
public record GetNutritionPlansByMemberQuery(long MemberId) : IRequest<List<NutritionPlanDto>>;
