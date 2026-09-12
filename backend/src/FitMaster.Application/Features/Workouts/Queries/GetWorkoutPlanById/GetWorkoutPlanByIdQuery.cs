using MediatR;

namespace FitMaster.Application.Features.Workouts.Queries.GetWorkoutPlanById;

public record GetWorkoutPlanByIdQuery(long Id) : IRequest<WorkoutPlanDto>;
