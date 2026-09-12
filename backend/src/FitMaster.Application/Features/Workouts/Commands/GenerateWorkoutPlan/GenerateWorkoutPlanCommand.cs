using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Workouts.Commands.GenerateWorkoutPlan;

/// <summary>
/// Generates a full workout plan for a member from their MemberProfile
/// (goal, fitness level, split type, training style, injuries). Archives the
/// member's current active plan, if any.
/// </summary>
public record GenerateWorkoutPlanCommand(long MemberId) : IRequest<Result<long>>;
