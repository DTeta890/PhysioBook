using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Configurations;

public class WalkInEntryConfiguration : IEntityTypeConfiguration<WalkInEntry>
{
    public void Configure(EntityTypeBuilder<WalkInEntry> builder)
    {
        builder.ToTable("walk_in_entries");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(w => w.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(w => w.PatientId)
            .HasColumnName("patient_id");

        builder.Property(w => w.PatientName)
            .HasColumnName("patient_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.PatientPhone)
            .HasColumnName("patient_phone")
            .HasMaxLength(50);

        builder.Property(w => w.TreatmentTypeId)
            .HasColumnName("treatment_type_id");

        builder.Property(w => w.ReasonForVisit)
            .HasColumnName("reason_for_visit")
            .HasMaxLength(500);

        builder.Property(w => w.Priority)
            .HasColumnName("priority")
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(w => w.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(w => w.CheckedInAt)
            .HasColumnName("checked_in_at")
            .IsRequired();

        builder.Property(w => w.CalledAt)
            .HasColumnName("called_at");

        builder.Property(w => w.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(w => w.AssignedTherapistId)
            .HasColumnName("assigned_therapist_id");

        builder.Property(w => w.ConvertedAppointmentId)
            .HasColumnName("converted_appointment_id");

        builder.Property(w => w.QueuePosition)
            .HasColumnName("queue_position")
            .IsRequired();

        builder.Property(w => w.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);

        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(w => w.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(w => new { w.TenantId, w.Status })
            .HasDatabaseName("idx_walk_in_entries_tenant_status")
            .HasFilter("status = 'waiting'");

        builder.HasIndex(w => new { w.TenantId, w.CheckedInAt })
            .HasDatabaseName("idx_walk_in_entries_tenant_checked_in");

        builder.HasIndex(w => new { w.TenantId, w.PatientId })
            .HasDatabaseName("idx_walk_in_entries_tenant_patient")
            .HasFilter("patient_id IS NOT NULL");

        // Foreign key to Tenant
        builder.HasOne(w => w.Tenant)
            .WithMany()
            .HasForeignKey(w => w.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key to TreatmentType
        builder.HasOne(w => w.TreatmentType)
            .WithMany()
            .HasForeignKey(w => w.TreatmentTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Foreign key to User (AssignedTherapist)
        builder.HasOne(w => w.AssignedTherapist)
            .WithMany()
            .HasForeignKey(w => w.AssignedTherapistId)
            .OnDelete(DeleteBehavior.SetNull);

        // Foreign key to Appointment (ConvertedAppointment)
        builder.HasOne(w => w.ConvertedAppointment)
            .WithMany()
            .HasForeignKey(w => w.ConvertedAppointmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
