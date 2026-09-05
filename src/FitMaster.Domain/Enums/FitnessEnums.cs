namespace FitMaster.Domain.Enums;

public enum FitnessGoal
{
    MuscleGain,
    WeightLoss,
    Endurance,
    GeneralFitness
}


public enum FitnessLevel
{
    Beginner,
    Intermediate,
    Advanced
}


public enum SplitType
{

    FullBody,

    
    UpperLower,

  
    PushPullLegs
}


public enum TrainingStyle
{
   
    Strength,

  
    Hypertrophy,


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
