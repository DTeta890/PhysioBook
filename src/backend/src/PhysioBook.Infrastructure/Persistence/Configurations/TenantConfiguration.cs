using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Slug)
            .HasColumnName("slug")
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(t => t.Slug).IsUnique();

        builder.Property(t => t.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Phone)
            .HasColumnName("phone")
            .HasMaxLength(50);

        builder.Property(t => t.Address)
            .HasColumnName("address");

        builder.Property(t => t.City)
            .HasColumnName("city")
            .HasMaxLength(100)
            .HasDefaultValue("Elbasan");

        builder.Property(t => t.Timezone)
            .HasColumnName("timezone")
            .HasMaxLength(50)
            .HasDefaultValue("Europe/Tirane");

        builder.Property(t => t.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(500);

        builder.Property(t => t.SubscriptionPlan)
            .HasColumnName("subscription_plan")
            .HasMaxLength(50)
            .HasDefaultValue("starter");

        builder.Property(t => t.SubscriptionStatus)
            .HasColumnName("subscription_status")
            .HasMaxLength(20)
            .HasDefaultValue("trial");

        builder.Property(t => t.TrialEndsAt)
            .HasColumnName("trial_ends_at");

        builder.Property(t => t.Settings)
            .HasColumnName("settings")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'");

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        builder.HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
