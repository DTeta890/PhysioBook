using Microsoft.EntityFrameworkCore;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<TreatmentType> TreatmentTypes { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<RecurringRule> RecurringRules { get; }
    DbSet<Patient> Patients { get; }
    DbSet<TreatmentNote> TreatmentNotes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
