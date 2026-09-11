using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;
using MediatR;

namespace FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlansByMember;

/// <summary>Full plan detail per entry (not a lightweight summary) - the history
/// screen renders every past plan's days/exercises inline, and a member's plan
/// count is small enough that this isn't a real payload concern.</summary>
public record GetWorkoutPlansByMemberQuery(long MemberId) : IRequest<List<WorkoutPlanDto>>;
