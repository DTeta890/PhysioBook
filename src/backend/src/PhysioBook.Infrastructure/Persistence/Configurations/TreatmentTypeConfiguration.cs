using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Configurations;

public class TreatmentTypeConfiguration : IEntityTypeConfiguration<TreatmentType>
{
    public void Configure(EntityTypeBuilder<TreatmentType> builder)
    {
        builder.ToTable("treatment_types");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(t => t.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasColumnName("description");

        builder.Property(t => t.DurationMinutes)
            .HasColumnName("duration_minutes")
            .HasDefaultValue(30)
            .IsRequired();

        builder.Property(t => t.Price)
            .HasColumnName("price")
            .HasPrecision(10, 2)
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(t => t.Color)
            .HasColumnName("color")
            .HasMaxLength(7);

        builder.Property(t => t.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        // Indexes matching SQL migration
        builder.HasIndex(t => t.TenantId)
            .HasDatabaseName("idx_treatment_types_tenant");

        builder.HasIndex(t => new { t.TenantId, t.IsActive })
            .HasDatabaseName("idx_treatment_types_tenant_active")
            .HasFilter("is_active = true");

        // Foreign key to Tenant
        builder.HasOne(t => t.Tenant)
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
