using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Application.Patients.Queries;

public sealed record SearchPatientsQuery(
    string SearchTerm,
    int Limit = 10) : IRequest<List<PatientSearchResult>>;

public sealed record PatientSearchResult(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string? Phone,
    string? Email,
    bool IsActive,
    double Similarity);

public sealed class SearchPatientsQueryHandler : IRequestHandler<SearchPatientsQuery, List<PatientSearchResult>>
{
    private readonly IApplicationDbContext _context;

    public SearchPatientsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PatientSearchResult>> Handle(SearchPatientsQuery request, CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm.Trim();

        var results = await _context.Patients
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Where(p =>
                EF.Functions.ILike(p.FirstName, $"%{searchTerm}%") ||
                EF.Functions.ILike(p.LastName, $"%{searchTerm}%") ||
                EF.Functions.TrigramsAreSimilar(p.FirstName, searchTerm) ||
                EF.Functions.TrigramsAreSimilar(p.LastName, searchTerm) ||
                (p.Phone != null && (
                    EF.Functions.ILike(p.Phone, $"%{searchTerm}%") ||
                    EF.Functions.TrigramsAreSimilar(p.Phone, searchTerm)
                ))
            )
            .OrderByDescending(p => EF.Functions.TrigramsSimilarity(p.LastName, searchTerm))
            .ThenByDescending(p => EF.Functions.TrigramsSimilarity(p.FirstName, searchTerm))
            .Take(request.Limit)
            .Select(p => new PatientSearchResult(
                p.Id,
                p.FirstName,
                p.LastName,
                p.FirstName + " " + p.LastName,
                p.Phone,
                p.Email,
                p.IsActive,
                (double)(EF.Functions.TrigramsSimilarity(p.FirstName, searchTerm) +
                         EF.Functions.TrigramsSimilarity(p.LastName, searchTerm))
            ))
            .ToListAsync(cancellationToken);

        return results;
    }
}
