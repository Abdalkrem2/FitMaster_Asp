using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlansByMember;

public class GetWorkoutPlansByMemberHandler(IApplicationDbContext db)
    : IRequestHandler<GetWorkoutPlansByMemberQuery, List<WorkoutPlanListItemDto>>
{
    public Task<List<WorkoutPlanListItemDto>> Handle(GetWorkoutPlansByMemberQuery request, CancellationToken cancellationToken)
        => db.WorkoutPlans
            .Where(w => w.MemberId == request.MemberId)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WorkoutPlanListItemDto(w.Id, w.Name, w.SplitType, w.Status, w.CreatedAt))
            .ToListAsync(cancellationToken);
}
