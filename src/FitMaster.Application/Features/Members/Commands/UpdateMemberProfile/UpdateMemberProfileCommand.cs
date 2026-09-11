using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;

namespace FitMaster.Application.Features.Members.Commands.UpdateMemberProfile;

public record UpdateMemberProfileCommand(
    long MemberId,
    FitnessGoal Goal,
    FitnessLevel FitnessLevel,
    SplitType SplitType,
    TrainingStyle? TrainingStyle,
    List<InjuryType>? Injuries,
    double? Weight,
    double? Height,
    int? Age,
    bool HasDiabetes,
    bool HasHeartConditions,
    bool HasHypertension,
    List<AllergyType>? Allergies) : IRequest<Result>;
