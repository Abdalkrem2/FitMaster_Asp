using FitMaster.Domain.Enums;

namespace FitMaster.Application.WorkoutGeneration;

/// <summary>One muscle an exercise targets, with its day-splitting group (if any) and role.</summary>
public record MuscleTarget(long MuscleId, MuscleGroup? Group, MuscleRole Role);

/// <summary>
/// A plain-data snapshot of an <see cref="FitMaster.Domain.Entities.Exercises.Exercise"/>
/// used by the generator - deliberately free of EF Core so <see cref="IExerciseSelector"/>
/// stays unit-testable without a database.
/// </summary>
public record ExerciseCandidate(Guid ExerciseId, DifficultyLevel? Difficulty, IReadOnlyList<MuscleTarget> Targets)
{
    /// <summary>How many distinct muscles this exercise loads (Primary + Secondary) - the
    /// compound/isolation signal: 4+ reads as a compound movement, fewer as isolation.</summary>
    public int CompoundScore => Targets.Count;
}
