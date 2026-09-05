using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Enums;
using FitMaster.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitMaster.Infrastructure.Persistence.Configurations;

public class MemberProfileConfiguration : IEntityTypeConfiguration<MemberProfile>
{
    public void Configure(EntityTypeBuilder<MemberProfile> builder)
    {
        builder.ToTable("member_profiles");

        // Shares its primary key with the owning User (one-to-one "MapsId" style).
        builder.HasKey(mp => mp.MemberId);
        builder.Property(mp => mp.MemberId).HasColumnName("member_id").ValueGeneratedNever();

        builder.Property(mp => mp.Goal).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(mp => mp.FitnessLevel).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(mp => mp.SplitType).HasColumnName("split_type").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(mp => mp.TrainingStyle).HasColumnName("training_style").HasConversion<string>().HasMaxLength(30);

        builder.Property(mp => mp.Weight);
        builder.Property(mp => mp.Height);
        builder.Property(mp => mp.Age);
        builder.Property(mp => mp.HasDiabetes).HasColumnName("has_diabetes");
        builder.Property(mp => mp.HasHeartConditions).HasColumnName("has_heart_conditions");
        builder.Property(mp => mp.HasHypertension).HasColumnName("has_hypertension");

        // Small value collections -> single JSON column each (see EnumListJsonConverter for why).
        builder.Property(mp => mp.Injuries).ConfigureAsJson<InjuryType>("injuries");
        builder.Property(mp => mp.Allergies).ConfigureAsJson<AllergyType>("allergies");
    }
}
