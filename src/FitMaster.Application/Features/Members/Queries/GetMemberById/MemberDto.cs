using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Members.Queries.GetMemberById;

public record MemberDto(
    long MemberId,
    string Phone,
    string FullName,
    string? ProfilePicture,
    FitnessGoal Goal,
    FitnessLevel FitnessLevel,
    SplitType SplitType,
    TrainingStyle? TrainingStyle,
    List<InjuryType> Injuries,
    double? Weight,
    double? Height,
    int? Age,
    bool HasDiabetes,
    bool HasHeartConditions,
    bool HasHypertension,
    List<AllergyType> Allergies);
