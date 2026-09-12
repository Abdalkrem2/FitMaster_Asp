using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Nutrition.Commands.ArchiveNutritionPlan;

public record ArchiveNutritionPlanCommand(long Id) : IRequest<Result>;
