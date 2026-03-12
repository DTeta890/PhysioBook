using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Configurations;

public class TreatmentPackageConfiguration : IEntityTypeConfiguration<TreatmentPackage>
{
    public void Configure(EntityTypeBuilder<TreatmentPackage> builder)
    {
        builder.ToTable("treatment_packages");

        builder.HasKey(tp => tp.Id);
        builder.Property(tp => tp.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(tp => tp.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(tp => tp.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(tp => tp.TreatmentTypeId)
            .HasColumnName("treatment_type_id")
            .IsRequired();

        builder.Property(tp => tp.TotalSessions)
            .HasColumnName("total_sessions")
            .IsRequired();

        builder.Property(tp => tp.Price)
            .HasColumnName("price")
            .HasPrecision(10, 2)
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(tp => tp.ValidityDays)
            .HasColumnName("validity_days");

        builder.Property(tp => tp.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(tp => tp.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(tp => tp.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(tp => tp.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(tp => tp.TenantId)
            .HasDatabaseName("idx_treatment_packages_tenant");

        builder.HasIndex(tp => new { tp.TenantId, tp.TreatmentTypeId })
            .HasDatabaseName("idx_treatment_packages_tenant_treatment_type");

        builder.HasIndex(tp => new { tp.TenantId, tp.IsActive })
            .HasDatabaseName("idx_treatment_packages_tenant_active")
            .HasFilter("is_active = true");

        // Foreign keys
        builder.HasOne(tp => tp.Tenant)
            .WithMany()
            .HasForeignKey(tp => tp.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tp => tp.TreatmentType)
            .WithMany()
            .HasForeignKey(tp => tp.TreatmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
