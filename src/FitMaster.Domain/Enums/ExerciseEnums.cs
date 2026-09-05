namespace FitMaster.Domain.Enums;

/// <summary>How hard an exercise is.</summary>
public enum DifficultyLevel
{
    Beginner,
    Intermediate,
    Advanced
}

/// <summary>Whether a muscle is the primary or secondary target of an exercise.</summary>
public enum MuscleRole
{
    Primary,
    Secondary
}

/// <summary>Broad body area a muscle belongs to.</summary>
public enum BodyRegion
{
    Upper,
    Lower,
    Core,
    Full
}

/// <summary>Classification of a Tag (e.g. movement category vs. required equipment).</summary>
public enum TagCategory
{
    Category,
    Equipment
}
