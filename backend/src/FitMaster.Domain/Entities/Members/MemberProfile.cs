using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Members;


public class MemberProfile
{
    public long MemberId { get; set; }

    public User Member { get; set; } = null!;


    public required FitnessGoal Goal { get; set; }

    public required FitnessLevel FitnessLevel { get; set; }

    public required SplitType SplitType { get; set; }

    public TrainingStyle? TrainingStyle { get; set; }

    // Existing members without this set default to Gym - the assumption the workout
    // generator has effectively been using all along (no equipment filtering at all).
    public EquipmentPreference EquipmentPreference { get; set; } = EquipmentPreference.Gym;

    public List<InjuryType> Injuries { get; set; } = new();

    // --- Nutrition-relevant health data --------------------------------------

    public double? Weight { get; set; }

    public double? Height { get; set; }

    public int? Age { get; set; }

    public bool HasDiabetes { get; set; }

    public bool HasHeartConditions { get; set; }

    public bool HasHypertension { get; set; }

    public List<AllergyType> Allergies { get; set; } = new();
}
