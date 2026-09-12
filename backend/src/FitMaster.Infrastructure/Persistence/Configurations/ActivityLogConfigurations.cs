using FitMaster.Domain.Entities.ActivityLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitMaster.Infrastructure.Persistence.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("activity_logs");

        builder.Property(a => a.PerformedById).HasColumnName("performed_by").IsRequired();
        builder.HasOne(a => a.PerformedBy).WithMany(u => u.ActivityLogsPerformed)
            .HasForeignKey(a => a.PerformedById).OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.Action).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(a => a.EntityType).HasColumnName("entity_type").HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.EntityId).HasColumnName("entity_id").IsRequired();
        builder.Property(a => a.Details);
        builder.Property(a => a.CreatedAt).HasColumnName("created_at").IsRequired();
    }
}
