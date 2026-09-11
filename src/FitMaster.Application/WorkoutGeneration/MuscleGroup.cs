namespace FitMaster.Application.WorkoutGeneration;

/// <summary>
/// A day-splitting muscle group. The seeded `movement_patterns` reference
/// data only has generic categories (strength/cardio/mobility/...), not
/// actual push/pull/squat patterns, so day-splitting and exercise targeting
/// are driven by real seeded `Muscle` names grouped here instead.
/// </summary>
public enum MuscleGroup
{
    Chest,
    Back,
    Shoulders,
    Biceps,
    Triceps,
    Quadriceps,
    Hamstrings,
    Glutes,
    Calves,
    Abs,
}

/// <summary>
/// Maps each <see cref="MuscleGroup"/> to the seeded `muscles.name` synonyms
/// that belong to it (the source exercise catalog uses several near-synonyms
/// for the same muscle, e.g. "chest"/"pectorals"/"upper chest").
/// </summary>
public static class MuscleGroupCatalog
{
    public static readonly IReadOnlyDictionary<MuscleGroup, IReadOnlyCollection<string>> SynonymsByGroup =
        new Dictionary<MuscleGroup, IReadOnlyCollection<string>>
        {
            [MuscleGroup.Chest] = ["chest", "pectorals", "upper chest"],
            [MuscleGroup.Back] = ["back", "lats", "latissimus dorsi", "upper back", "rhomboids", "trapezius", "traps"],
            [MuscleGroup.Shoulders] = ["shoulders", "deltoids", "delts", "rear deltoids", "rotator cuff"],
            [MuscleGroup.Biceps] = ["biceps", "brachialis"],
            [MuscleGroup.Triceps] = ["triceps"],
            [MuscleGroup.Quadriceps] = ["quadriceps", "quads"],
            [MuscleGroup.Hamstrings] = ["hamstrings"],
            [MuscleGroup.Glutes] = ["glutes"],
            [MuscleGroup.Calves] = ["calves", "soleus"],
            [MuscleGroup.Abs] = ["abs", "abdominals", "core", "obliques", "lower abs"],
        };
}
