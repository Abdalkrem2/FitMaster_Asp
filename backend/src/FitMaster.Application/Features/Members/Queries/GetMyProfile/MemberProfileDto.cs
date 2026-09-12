using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Members.Queries.GetMyProfile;

public record MemberProfileDto(
    FitnessGoal Goal,
    FitnessLevel FitnessLevel,
    SplitType SplitType,
    TrainingStyle? TrainingStyle,
    EquipmentPreference EquipmentPreference,
    List<InjuryType> Injuries,
    double? Weight,
    double? Height,
    int? Age,
    bool HasDiabetes,
    bool HasHeartConditions,
    bool HasHypertension,
    List<AllergyType> Allergies);
