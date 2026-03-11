using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(a => a.TherapistId)
            .HasColumnName("therapist_id")
            .IsRequired();

        builder.Property(a => a.PatientId)
            .HasColumnName("patient_id");

        builder.Property(a => a.TreatmentTypeId)
            .HasColumnName("treatment_type_id")
            .IsRequired();

        builder.Property(a => a.PatientName)
            .HasColumnName("patient_name")
            .HasMaxLength(255);

        builder.Property(a => a.PatientPhone)
            .HasColumnName("patient_phone")
            .HasMaxLength(50);

        builder.Property(a => a.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(a => a.EndTime)
            .HasColumnName("end_time")
            .IsRequired();

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Notes)
            .HasColumnName("notes");

        builder.Property(a => a.CancellationReason)
            .HasColumnName("cancellation_reason");

        builder.Property(a => a.IsWalkIn)
            .HasColumnName("is_walk_in")
            .HasDefaultValue(false);

        builder.Property(a => a.Color)
            .HasColumnName("color")
            .HasMaxLength(7);

        builder.Property(a => a.RecurringRuleId)
            .HasColumnName("recurring_rule_id");

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        // Indexes matching SQL migration
        builder.HasIndex(a => new { a.TenantId, a.TherapistId, a.StartTime })
            .HasDatabaseName("idx_appointments_tenant_therapist_date");

        builder.HasIndex(a => new { a.TenantId, a.StartTime })
            .HasDatabaseName("idx_appointments_tenant_date");

        builder.HasIndex(a => new { a.TenantId, a.PatientId })
            .HasDatabaseName("idx_appointments_tenant_patient")
            .HasFilter("patient_id IS NOT NULL");

        builder.HasIndex(a => new { a.TenantId, a.Status })
            .HasDatabaseName("idx_appointments_tenant_status")
            .HasFilter("status NOT IN ('completed', 'cancelled')");

        builder.HasIndex(a => a.RecurringRuleId)
            .HasDatabaseName("idx_appointments_recurring_rule")
            .HasFilter("recurring_rule_id IS NOT NULL");

        // Foreign key to Tenant
        builder.HasOne(a => a.Tenant)
            .WithMany()
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key to User (Therapist)
        builder.HasOne(a => a.Therapist)
            .WithMany()
            .HasForeignKey(a => a.TherapistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key to TreatmentType
        builder.HasOne(a => a.TreatmentType)
            .WithMany()
            .HasForeignKey(a => a.TreatmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Foreign key to RecurringRule
        builder.HasOne(a => a.RecurringRule)
            .WithMany(r => r.Appointments)
            .HasForeignKey(a => a.RecurringRuleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
