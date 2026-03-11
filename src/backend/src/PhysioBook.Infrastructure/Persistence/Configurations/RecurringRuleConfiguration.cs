using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Configurations;

public class RecurringRuleConfiguration : IEntityTypeConfiguration<RecurringRule>
{
    public void Configure(EntityTypeBuilder<RecurringRule> builder)
    {
        builder.ToTable("recurring_rules");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(r => r.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(r => r.TherapistId)
            .HasColumnName("therapist_id")
            .IsRequired();

        builder.Property(r => r.PatientId)
            .HasColumnName("patient_id");

        builder.Property(r => r.TreatmentTypeId)
            .HasColumnName("treatment_type_id")
            .IsRequired();

        builder.Property(r => r.PatientName)
            .HasColumnName("patient_name")
            .HasMaxLength(255);

        builder.Property(r => r.PatientPhone)
            .HasColumnName("patient_phone")
            .HasMaxLength(50);

        builder.Property(r => r.Frequency)
            .HasColumnName("frequency")
            .HasMaxLength(20)
            .HasDefaultValue("weekly")
            .IsRequired();

        builder.Property(r => r.DayOfWeek)
            .HasColumnName("day_of_week")
            .IsRequired();

        builder.Property(r => r.StartTimeOfDay)
            .HasColumnName("start_time_of_day")
            .HasColumnType("time")
            .IsRequired();

        builder.Property(r => r.EndTimeOfDay)
            .HasColumnName("end_time_of_day")
            .HasColumnType("time")
            .IsRequired();

        builder.Property(r => r.StartsFrom)
            .HasColumnName("starts_from")
            .IsRequired();

        builder.Property(r => r.EndsAt)
            .HasColumnName("ends_at");

        builder.Property(r => r.MaxOccurrences)
            .HasColumnName("max_occurrences");

        builder.Property(r => r.Notes)
            .HasColumnName("notes");

        builder.Property(r => r.Color)
            .HasColumnName("color")
            .HasMaxLength(7);

        builder.Property(r => r.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(r => r.TenantId)
            .HasDatabaseName("idx_recurring_rules_tenant");

        builder.HasIndex(r => new { r.TenantId, r.TherapistId })
            .HasDatabaseName("idx_recurring_rules_tenant_therapist");

        builder.HasIndex(r => new { r.TenantId, r.IsActive })
            .HasDatabaseName("idx_recurring_rules_active")
            .HasFilter("is_active = true");

        // Foreign keys
        builder.HasOne(r => r.Tenant)
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Therapist)
            .WithMany()
            .HasForeignKey(r => r.TherapistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.TreatmentType)
            .WithMany()
            .HasForeignKey(r => r.TreatmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
