using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Common;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentTenantService _tenantService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentTenantService tenantService,
        IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _tenantService = tenantService;
        _dateTimeProvider = dateTimeProvider;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TreatmentType> TreatmentTypes => Set<TreatmentType>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<RecurringRule> RecurringRules => Set<RecurringRule>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<TreatmentNote> TreatmentNotes => Set<TreatmentNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global tenant filter on all entities inheriting BaseEntity (which have TenantId)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(
                        EntityFilterExpression.CreateTenantFilter(entityType.ClrType, _tenantService));
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = _dateTimeProvider.Now;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.TenantId = _tenantService.TenantId;
                    if (entry.Entity.Id == Guid.Empty)
                        entry.Entity.Id = Guid.NewGuid();
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }

        // RLS session variable is set by TenantConnectionInterceptor at connection level

        return await base.SaveChangesAsync(cancellationToken);
    }
}
