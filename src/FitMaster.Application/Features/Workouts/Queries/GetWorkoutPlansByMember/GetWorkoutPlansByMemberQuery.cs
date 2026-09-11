using MediatR;

namespace FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlansByMember;

public record GetWorkoutPlansByMemberQuery(long MemberId) : IRequest<List<WorkoutPlanListItemDto>>;
