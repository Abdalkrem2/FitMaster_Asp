using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;

namespace FitMaster.Application.Features.Members.Commands.CreateMember;

/// <summary>
/// Registers a brand-new gym member: creates the User account (with the Member
/// role) and its fitness/health profile in one operation. Typically used by
/// front-desk staff signing someone up in person.
/// </summary>
public record CreateMemberCommand(
    string Phone,
    string Password,
    string FullName,
    FitnessGoal Goal,
    FitnessLevel FitnessLevel,
    SplitType SplitType,
    List<InjuryType>? Injuries,
    double? Weight,
    double? Height,
    int? Age,
    bool HasDiabetes,
    bool HasHeartConditions,
    bool HasHypertension,
    List<AllergyType>? Allergies) : IRequest<Result<long>>;
