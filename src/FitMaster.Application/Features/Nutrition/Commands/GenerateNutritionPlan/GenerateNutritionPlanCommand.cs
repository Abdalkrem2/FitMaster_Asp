using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Nutrition.Commands.GenerateNutritionPlan;

/// <summary>
/// Generates a full nutrition plan for a member: calorie/macro targets computed
/// deterministically from their profile, meal content generated via AI. Archives
/// the member's current active plan, if any.
/// </summary>
public record GenerateNutritionPlanCommand(long MemberId) : IRequest<Result<long>>;
