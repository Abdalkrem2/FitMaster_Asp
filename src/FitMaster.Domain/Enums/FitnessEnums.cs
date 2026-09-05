namespace FitMaster.Domain.Enums;

/// <summary>The member's primary fitness objective.</summary>
public enum FitnessGoal
{
    MuscleGain,
    WeightLoss,
    Endurance,
    GeneralFitness
}

/// <summary>Self-reported / assessed training experience.</summary>
public enum FitnessLevel
{
    Beginner,
    Intermediate,
    Advanced
}

/// <summary>Weekly training split used to generate a WorkoutPlan.</summary>
public enum SplitType
{
    /// <summary>3 days/week - full body each session.</summary>
    FullBody,

    /// <summary>4 days/week - alternating upper/lower.</summary>
    UpperLower,

    /// <summary>6 days/week - push / pull / legs.</summary>
    PushPullLegs
}

/// <summary>Rep-range / intensity style driving set & rep generation.</summary>
public enum TrainingStyle
{
    /// <summary>3-5 reps, heavy.</summary>
    Strength,

    /// <summary>8-12 reps, moderate.</summary>
    Hypertrophy,

    /// <summary>15+ reps, light and fast - conditioning focus.</summary>
    Circuit
}

/// <summary>Known injuries a member has reported, used to filter unsafe exercises.</summary>
public enum InjuryType
{
    Knee,
    Shoulder,
    LowerBack,
    UpperBack,
    Wrist,
    Ankle,
    Neck,
    Elbow,
    Hip
}

/// <summary>Food allergies/intolerances a member has reported.</summary>
public enum AllergyType
{
    Gluten,
    Lactose,
    Nuts,
    Eggs,
    Shellfish,
    Soy
}

/// <summary>General active/inactive status for a member.</summary>
public enum MemberStatus
{
    Active,
    Inactive
}
