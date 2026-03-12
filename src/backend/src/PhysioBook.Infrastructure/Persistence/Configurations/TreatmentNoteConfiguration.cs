using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Configurations;

public class TreatmentNoteConfiguration : IEntityTypeConfiguration<TreatmentNote>
{
    public void Configure(EntityTypeBuilder<TreatmentNote> builder)
    {
        builder.ToTable("treatment_notes");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(t => t.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(t => t.AppointmentId)
            .HasColumnName("appointment_id")
            .IsRequired();

        builder.Property(t => t.PatientId)
            .HasColumnName("patient_id");

        builder.Property(t => t.TherapistId)
            .HasColumnName("therapist_id")
            .IsRequired();

        // SOAP
        builder.Property(t => t.Subjective)
            .HasColumnName("subjective");

        builder.Property(t => t.Objective)
            .HasColumnName("objective");

        builder.Property(t => t.Assessment)
            .HasColumnName("assessment");

        builder.Property(t => t.Plan)
            .HasColumnName("plan");

        // Additional
        builder.Property(t => t.Diagnosis)
            .HasColumnName("diagnosis");

        builder.Property(t => t.TreatmentProvided)
            .HasColumnName("treatment_provided");

        builder.Property(t => t.PainLevelBefore)
            .HasColumnName("pain_level_before");

        builder.Property(t => t.PainLevelAfter)
            .HasColumnName("pain_level_after");

        builder.Property(t => t.RangeOfMotionNotes)
            .HasColumnName("range_of_motion_notes");

        builder.Property(t => t.ExercisesPrescribed)
            .HasColumnName("exercises_prescribed");

        builder.Property(t => t.FollowUpInstructions)
            .HasColumnName("follow_up_instructions");

        builder.Property(t => t.IsSigned)
            .HasColumnName("is_signed")
            .HasDefaultValue(false);

        builder.Property(t => t.SignedAt)
            .HasColumnName("signed_at");

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(t => new { t.TenantId, t.AppointmentId })
            .HasDatabaseName("idx_treatment_notes_tenant_appointment");

        builder.HasIndex(t => new { t.TenantId, t.PatientId })
            .HasDatabaseName("idx_treatment_notes_tenant_patient")
            .HasFilter("patient_id IS NOT NULL");

        builder.HasIndex(t => new { t.TenantId, t.TherapistId })
            .HasDatabaseName("idx_treatment_notes_tenant_therapist");

        // Unique constraint: one treatment note per appointment
        builder.HasIndex(t => t.AppointmentId)
            .HasDatabaseName("idx_treatment_notes_appointment_unique")
            .IsUnique();

        // Foreign key to Tenant
        builder.HasOne(t => t.Tenant)
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key to Appointment (one-to-one)
        builder.HasOne(t => t.Appointment)
            .WithOne(a => a.TreatmentNote)
            .HasForeignKey<TreatmentNote>(t => t.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key to Patient
        builder.HasOne(t => t.Patient)
            .WithMany()
            .HasForeignKey(t => t.PatientId)
            .OnDelete(DeleteBehavior.SetNull);

        // Foreign key to User (Therapist)
        builder.HasOne(t => t.Therapist)
            .WithMany()
            .HasForeignKey(t => t.TherapistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
