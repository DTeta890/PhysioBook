using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(p => p.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasColumnName("email")
            .HasMaxLength(255);

        builder.Property(p => p.Phone)
            .HasColumnName("phone")
            .HasMaxLength(50);

        builder.Property(p => p.DateOfBirth)
            .HasColumnName("date_of_birth");

        builder.Property(p => p.Gender)
            .HasColumnName("gender")
            .HasMaxLength(10);

        builder.Property(p => p.Address)
            .HasColumnName("address");

        builder.Property(p => p.City)
            .HasColumnName("city")
            .HasMaxLength(100);

        builder.Property(p => p.EmergencyContactName)
            .HasColumnName("emergency_contact_name")
            .HasMaxLength(200);

        builder.Property(p => p.EmergencyContactPhone)
            .HasColumnName("emergency_contact_phone")
            .HasMaxLength(50);

        builder.Property(p => p.MedicalHistory)
            .HasColumnName("medical_history");

        builder.Property(p => p.Allergies)
            .HasColumnName("allergies");

        builder.Property(p => p.Notes)
            .HasColumnName("notes");

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        // Ignore computed property
        builder.Ignore(p => p.FullName);

        // Indexes matching SQL migration
        builder.HasIndex(p => new { p.TenantId, p.LastName, p.FirstName })
            .HasDatabaseName("idx_patients_tenant_name");

        builder.HasIndex(p => new { p.TenantId, p.Phone })
            .HasDatabaseName("idx_patients_tenant_phone")
            .HasFilter("phone IS NOT NULL");

        builder.HasIndex(p => new { p.TenantId, p.Email })
            .HasDatabaseName("idx_patients_tenant_email")
            .HasFilter("email IS NOT NULL");

        builder.HasIndex(p => new { p.TenantId, p.IsActive })
            .HasDatabaseName("idx_patients_tenant_active")
            .HasFilter("is_active = true");

        // Foreign key to Tenant
        builder.HasOne(p => p.Tenant)
            .WithMany()
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation to Appointments
        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
