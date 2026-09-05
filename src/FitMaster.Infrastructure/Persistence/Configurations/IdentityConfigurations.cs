using FitMaster.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitMaster.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(u => u.Phone).HasMaxLength(30).IsRequired();
        builder.HasIndex(u => u.Phone).IsUnique();

        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(200).IsRequired();
        builder.Property(u => u.ProfilePicture).HasColumnName("profile_picture");
        builder.Property(u => u.Gender).HasMaxLength(20);
        builder.Property(u => u.AdminRoleAssignedAt).HasColumnName("admin_role_assigned_at");
        builder.Property(u => u.IsActivated).HasColumnName("is_activated");
        builder.Property(u => u.Deleted).HasColumnName("deleted");
        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(u => u.CreatedBy)
            .WithMany()
            .HasForeignKey(u => u.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("user_roles"));

        builder.HasOne(u => u.MemberProfile)
            .WithOne(mp => mp.Member)
            .HasForeignKey<Domain.Entities.Members.MemberProfile>(mp => mp.MemberId);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.Property(r => r.RoleName)
            .HasColumnName("role_name")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}
