using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Nutrition.Queries.GetNutritionPlansByMember;

public class GetNutritionPlansByMemberHandler(IApplicationDbContext db)
    : IRequestHandler<GetNutritionPlansByMemberQuery, List<NutritionPlanListItemDto>>
{
    public Task<List<NutritionPlanListItemDto>> Handle(GetNutritionPlansByMemberQuery request, CancellationToken cancellationToken)
        => db.NutritionPlans
            .Where(p => p.MemberId == request.MemberId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new NutritionPlanListItemDto(p.Id, p.Goal, p.DailyCalories, p.Status, p.CreatedAt))
            .ToListAsync(cancellationToken);
}
