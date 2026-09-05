using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Members;

/// <summary>
/// Fitness & health profile for a member. Shares its primary key with the
/// owning <see cref="User"/> (one-to-one, "MapsId" relationship in the original schema).
/// </summary>
public class MemberProfile
{
    /// <summary>Same value as the owning User's Id.</summary>
    public long MemberId { get; set; }

    public User Member { get; set; } = null!;

    public required FitnessGoal Goal { get; set; }

    public required FitnessLevel FitnessLevel { get; set; }

    public required SplitType SplitType { get; set; }

    public TrainingStyle? TrainingStyle { get; set; }

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
