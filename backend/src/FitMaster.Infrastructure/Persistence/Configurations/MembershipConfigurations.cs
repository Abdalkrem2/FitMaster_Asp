using FitMaster.Domain.Entities.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitMaster.Infrastructure.Persistence.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("packages");

        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Price).HasPrecision(10, 2).IsRequired();
        builder.Property(p => p.DurationInDays).HasColumnName("duration_days").IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Deleted);
    }
}

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("memberships");

        builder.Property(m => m.MemberId).HasColumnName("member_id").IsRequired();
        builder.HasOne(m => m.Member).WithMany(u => u.Memberships)
            .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.PackageId).HasColumnName("package_id").IsRequired();
        builder.HasOne(m => m.Package).WithMany(p => p.Memberships)
            .HasForeignKey(m => m.PackageId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(m => m.EndDate).HasColumnName("end_date").IsRequired();
        builder.Property(m => m.Price).HasPrecision(10, 2);
        builder.Property(m => m.Debt).HasPrecision(10, 2);
        builder.Property(m => m.Description).HasMaxLength(1000);
        builder.Property(m => m.CreatedAt).HasColumnName("created_at");
        builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");
    }
}

public class RevenueConfiguration : IEntityTypeConfiguration<Revenue>
{
    public void Configure(EntityTypeBuilder<Revenue> builder)
    {
        builder.ToTable("revenues");

        builder.Property(r => r.MemberId).HasColumnName("member_id").IsRequired();
        builder.HasOne(r => r.Member).WithMany()
            .HasForeignKey(r => r.MemberId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.MembershipId).HasColumnName("membership_id").IsRequired();
        builder.HasOne(r => r.Membership).WithMany(m => m.Revenues)
            .HasForeignKey(r => r.MembershipId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.CreatedById).HasColumnName("created_by").IsRequired();
        builder.HasOne(r => r.CreatedBy).WithMany()
            .HasForeignKey(r => r.CreatedById).OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.Amount).HasPrecision(10, 2).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(1000);
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
    }
}
