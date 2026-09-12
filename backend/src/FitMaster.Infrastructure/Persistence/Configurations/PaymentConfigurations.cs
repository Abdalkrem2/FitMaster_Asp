using FitMaster.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitMaster.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.Property(p => p.MemberId).HasColumnName("member_id").IsRequired();
        builder.HasOne(p => p.Member).WithMany(u => u.Payments)
            .HasForeignKey(p => p.MemberId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.PackageId).HasColumnName("package_id").IsRequired();
        builder.HasOne(p => p.Package).WithMany(pkg => pkg.Payments)
            .HasForeignKey(p => p.PackageId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Amount).HasPrecision(10, 2).IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.GatewayReference).HasColumnName("gateway_reference").HasMaxLength(255);

        builder.Property(p => p.MembershipId).HasColumnName("membership_id");
        builder.HasOne(p => p.Membership).WithMany()
            .HasForeignKey(p => p.MembershipId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.CompletedAt).HasColumnName("completed_at");

        // Correlates an incoming webhook event back to this row without a second
        // round trip - looked up by GatewayReference (the Stripe checkout session id).
        builder.HasIndex(p => p.GatewayReference);
    }
}
