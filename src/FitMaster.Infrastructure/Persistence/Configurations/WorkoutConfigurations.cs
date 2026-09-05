using FitMaster.Domain.Entities.Workouts;
using FitMaster.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitMaster.Infrastructure.Persistence.Configurations;

public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
{
    public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
    {
        builder.ToTable("workout_plans");

        builder.Property(w => w.Name).HasMaxLength(200);
        builder.Property(w => w.Goal).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(w => w.Level).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(w => w.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(w => w.SplitType).HasColumnName("split_type").HasConversion<string>().HasMaxLength(30).IsRequired();

        builder.Property(w => w.MemberId).HasColumnName("member_id").IsRequired();
        builder.HasOne(w => w.Member).WithMany(u => u.WorkoutPlans)
            .HasForeignKey(w => w.MemberId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(w => w.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasMany(w => w.WorkoutDays).WithOne(d => d.WorkoutPlan)
            .HasForeignKey(d => d.WorkoutPlanId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class WorkoutDayConfiguration : IEntityTypeConfiguration<WorkoutDay>
{
    public void Configure(EntityTypeBuilder<WorkoutDay> builder)
    {
        builder.ToTable("workout_days");

        builder.Property(d => d.WorkoutPlanId).HasColumnName("workout_plan_id").IsRequired();
        builder.Property(d => d.MuscleGroupLabel).HasColumnName("muscle_group_label").HasMaxLength(200).IsRequired();
        builder.Property(d => d.DayNumber).HasColumnName("day_number").IsRequired();

        builder.HasMany(d => d.WorkoutExercises).WithOne(e => e.WorkoutDay)
            .HasForeignKey(e => e.WorkoutDayId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
    {
        builder.ToTable("workout_exercises");

        builder.Property(e => e.WorkoutDayId).HasColumnName("workout_day_id").IsRequired();

        // Same binary(16) representation as Exercise.Id - see GuidToBytesConverter.
        builder.Property(e => e.ExerciseId)
            .HasColumnName("exercise_id")
            .HasConversion(new GuidToBytesConverter())
            .HasColumnType("binary(16)")
            .IsRequired();
        builder.HasOne(e => e.Exercise).WithMany()
            .HasForeignKey(e => e.ExerciseId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.OrderIndex).HasColumnName("order_index").IsRequired();
        builder.Property(e => e.Sets);
        builder.Property(e => e.Reps);
        builder.Property(e => e.RepsMax).HasColumnName("reps_max");
        builder.Property(e => e.DurationSeconds).HasColumnName("duration_seconds");
    }
}
