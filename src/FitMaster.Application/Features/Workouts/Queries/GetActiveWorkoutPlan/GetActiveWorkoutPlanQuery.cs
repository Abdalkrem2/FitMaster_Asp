using FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;
using MediatR;

namespace FitMaster.Application.Features.Workouts.Queries.GetActiveWorkoutPlan;

public record GetActiveWorkoutPlanQuery(long MemberId) : IRequest<WorkoutPlanDto>;
