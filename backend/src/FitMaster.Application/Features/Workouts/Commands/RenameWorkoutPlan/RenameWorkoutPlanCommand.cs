using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Workouts.Commands.RenameWorkoutPlan;

public record RenameWorkoutPlanCommand(long Id, string Name) : IRequest<Result>;
