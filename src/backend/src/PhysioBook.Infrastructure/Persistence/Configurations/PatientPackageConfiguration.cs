using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Configurations;

public class PatientPackageConfiguration : IEntityTypeConfiguration<PatientPackage>
{
    public void Configure(EntityTypeBuilder<PatientPackage> builder)
    {
        builder.ToTable("patient_packages");

        builder.HasKey(pp => pp.Id);
        builder.Property(pp => pp.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(pp => pp.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(pp => pp.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(pp => pp.TreatmentPackageId)
            .HasColumnName("treatment_package_id")
            .IsRequired();

        builder.Property(pp => pp.SessionsUsed)
            .HasColumnName("sessions_used")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(pp => pp.PurchasedAt)
            .HasColumnName("purchased_at")
            .IsRequired();

        builder.Property(pp => pp.ExpiresAt)
            .HasColumnName("expires_at");

        builder.Property(pp => pp.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasDefaultValue("active")
            .IsRequired();

        builder.Property(pp => pp.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);

        builder.Property(pp => pp.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(pp => pp.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(pp => pp.TenantId)
            .HasDatabaseName("idx_patient_packages_tenant");

        builder.HasIndex(pp => new { pp.TenantId, pp.PatientId })
            .HasDatabaseName("idx_patient_packages_tenant_patient");

        builder.HasIndex(pp => new { pp.TenantId, pp.Status })
            .HasDatabaseName("idx_patient_packages_tenant_status")
            .HasFilter("status = 'active'");

        // Foreign keys
        builder.HasOne(pp => pp.Tenant)
            .WithMany()
            .HasForeignKey(pp => pp.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.TreatmentPackage)
            .WithMany()
            .HasForeignKey(pp => pp.TreatmentPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore computed property
        builder.Ignore(pp => pp.SessionsRemaining);
    }
}
