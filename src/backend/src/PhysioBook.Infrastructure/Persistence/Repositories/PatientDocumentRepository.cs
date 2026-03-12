using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Infrastructure.Persistence.Repositories;

public sealed class PatientDocumentRepository : IPatientDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public PatientDocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<PatientDocument>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<(List<PatientDocument> Items, int TotalCount)> GetByPatientIdAsync(
        Guid patientId,
        string? category,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<PatientDocument>()
            .AsNoTracking()
            .Where(d => d.PatientId == patientId);

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(d => d.Category == category.ToLowerInvariant());
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(PatientDocument document, CancellationToken cancellationToken = default)
    {
        _context.Set<PatientDocument>().Add(document);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(PatientDocument document, CancellationToken cancellationToken = default)
    {
        // Re-attach if the entity was loaded with AsNoTracking
        var tracked = _context.Set<PatientDocument>().Local.FirstOrDefault(d => d.Id == document.Id);
        if (tracked is null)
        {
            _context.Set<PatientDocument>().Attach(document);
        }

        _context.Set<PatientDocument>().Remove(tracked ?? document);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
