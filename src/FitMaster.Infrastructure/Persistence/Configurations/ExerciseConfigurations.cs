using FitMaster.Domain.Entities.Exercises;
using FitMaster.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitMaster.Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("exercises");

        // Raw 16-byte id, exactly like the original MySQL BINARY(16) - see GuidToBytesConverter.
        builder.Property(e => e.Id)
            .HasConversion(new GuidToBytesConverter())
            .HasColumnType("binary(16)")
            .ValueGeneratedNever();

        builder.Property(e => e.Code).HasMaxLength(10);
        builder.HasIndex(e => e.Code).IsUnique();

        builder.Property(e => e.DifficultyLevel).HasColumnName("difficulty_level").HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.IsArchived).HasColumnName("is_archived");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

        // Many-to-many, explicit join table names/columns matching the original schema.
        //
        // Important UsingEntity quirk: HasConversion/HasColumnType can't be chained
        // directly after HasForeignKey(string) - the shadow FK property itself needs
        // to be configured separately via the third (join entity) delegate below.
        builder.HasMany(e => e.MovementPatterns).WithMany(p => p.Exercises)
            .UsingEntity<Dictionary<string, object>>(
                "exercise_patterns",
                j => j.HasOne<MovementPattern>().WithMany().HasForeignKey("pattern_id"),
                j => j.HasOne<Exercise>().WithMany().HasForeignKey("exercise_id"),
                j =>
                {
                    j.Property<Guid>("exercise_id")
                        .HasConversion(new GuidToBytesConverter())
                        .HasColumnType("binary(16)");
                    j.HasKey("exercise_id", "pattern_id");
                });

        builder.HasMany(e => e.Tags).WithMany(t => t.Exercises)
            .UsingEntity<Dictionary<string, object>>(
                "exercise_tags",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("tag_id"),
                j => j.HasOne<Exercise>().WithMany().HasForeignKey("exercise_id"),
                j =>
                {
                    j.Property<Guid>("exercise_id")
                        .HasConversion(new GuidToBytesConverter())
                        .HasColumnType("binary(16)");
                    j.HasKey("exercise_id", "tag_id");
                });

        builder.HasMany(e => e.ExerciseMuscles).WithOne(em => em.Exercise)
            .HasForeignKey(em => em.ExerciseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.ExerciseEquipments).WithOne(ee => ee.Exercise)
            .HasForeignKey(ee => ee.ExerciseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Translations).WithOne(t => t.Exercise)
            .HasForeignKey(t => t.ExerciseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Media).WithOne(m => m.Exercise)
            .HasForeignKey(m => m.ExerciseId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class MuscleConfiguration : IEntityTypeConfiguration<Muscle>
{
    public void Configure(EntityTypeBuilder<Muscle> builder)
    {
        builder.ToTable("muscles");
        builder.Property(m => m.Name).HasMaxLength(100).IsRequired();
        builder.Property(m => m.BodyRegion).HasColumnName("body_region").HasConversion<string>().HasMaxLength(50).IsRequired();
    }
}

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("equipment");
        builder.Property(e => e.Name).HasMaxLength(255).IsRequired();
        builder.HasIndex(e => e.Name).IsUnique();
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        builder.Property(t => t.Category).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
    }
}

public class MovementPatternConfiguration : IEntityTypeConfiguration<MovementPattern>
{
    public void Configure(EntityTypeBuilder<MovementPattern> builder)
    {
        builder.ToTable("movement_patterns");
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
    }
}

public class ExerciseMuscleConfiguration : IEntityTypeConfiguration<ExerciseMuscle>
{
    public void Configure(EntityTypeBuilder<ExerciseMuscle> builder)
    {
        builder.ToTable("exercise_muscles");
        builder.HasKey(em => new { em.ExerciseId, em.MuscleId });

        builder.Property(em => em.ExerciseId)
            .HasColumnName("exercise_id")
            .HasConversion(new GuidToBytesConverter())
            .HasColumnType("binary(16)");
        builder.Property(em => em.MuscleId).HasColumnName("muscle_id");

        builder.HasOne(em => em.Muscle).WithMany(m => m.ExerciseMuscles)
            .HasForeignKey(em => em.MuscleId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(em => em.Role).HasConversion<string>().HasMaxLength(50).IsRequired();
    }
}

public class ExerciseEquipmentConfiguration : IEntityTypeConfiguration<ExerciseEquipment>
{
    public void Configure(EntityTypeBuilder<ExerciseEquipment> builder)
    {
        builder.ToTable("exercise_equipment");
        builder.HasKey(ee => new { ee.ExerciseId, ee.EquipmentId });

        builder.Property(ee => ee.ExerciseId)
            .HasColumnName("exercise_id")
            .HasConversion(new GuidToBytesConverter())
            .HasColumnType("binary(16)");
        builder.Property(ee => ee.EquipmentId).HasColumnName("equipment_id");

        builder.HasOne(ee => ee.Equipment).WithMany(e => e.ExerciseEquipments)
            .HasForeignKey(ee => ee.EquipmentId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(ee => ee.IsRequired).HasColumnName("is_required");
    }
}

public class ExerciseTranslationConfiguration : IEntityTypeConfiguration<ExerciseTranslation>
{
    public void Configure(EntityTypeBuilder<ExerciseTranslation> builder)
    {
        builder.ToTable("exercise_translations");

        builder.Property(t => t.ExerciseId)
            .HasColumnName("exercise_id")
            .HasConversion(new GuidToBytesConverter())
            .HasColumnType("binary(16)")
            .IsRequired();

        builder.Property(t => t.Name);
        builder.Property(t => t.Description);
        builder.Property(t => t.Instructions);
        builder.Property(t => t.AudioCueUrl).HasColumnName("audio_cue_url").HasMaxLength(500);
        builder.Property(t => t.Locale).HasMaxLength(10);
    }
}

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("media_assets");

        builder.Property(m => m.Id)
            .HasConversion(new GuidToBytesConverter())
            .HasColumnType("binary(16)")
            .ValueGeneratedNever();

        builder.Property(m => m.Url).HasMaxLength(500);
        builder.Property(m => m.Type).HasMaxLength(50);
        builder.Property(m => m.LicenseType).HasColumnName("license_type").HasMaxLength(50);
        builder.Property(m => m.AttributionText).HasColumnName("attribution_text").HasMaxLength(500);
        builder.Property(m => m.CreatedAt).HasColumnName("created_at");
    }
}

public class ExerciseMediaConfiguration : IEntityTypeConfiguration<ExerciseMedia>
{
    public void Configure(EntityTypeBuilder<ExerciseMedia> builder)
    {
        builder.ToTable("exercise_media");

        builder.Property(m => m.ExerciseId)
            .HasColumnName("exercise_id")
            .HasConversion(new GuidToBytesConverter())
            .HasColumnType("binary(16)")
            .IsRequired();

        builder.Property(m => m.MediaAssetId)
            .HasColumnName("media_asset_id")
            .HasConversion(new GuidToBytesConverter())
            .HasColumnType("binary(16)")
            .IsRequired();
        builder.HasOne(m => m.MediaAsset).WithMany(a => a.ExerciseMedia)
            .HasForeignKey(m => m.MediaAssetId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.Role).HasMaxLength(50);
        builder.Property(m => m.Sex).HasMaxLength(50);
        builder.Property(m => m.ViewAngle).HasColumnName("view_angle").HasMaxLength(50);
    }
}
