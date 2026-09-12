using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Workouts.Commands.ArchiveWorkoutPlan;

public record ArchiveWorkoutPlanCommand(long Id) : IRequest<Result>;
