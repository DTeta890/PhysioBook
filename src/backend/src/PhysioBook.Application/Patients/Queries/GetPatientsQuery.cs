using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Patients.DTOs;

namespace PhysioBook.Application.Patients.Queries;

public sealed record GetPatientsQuery(
    string? Search,
    bool? IsActive,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<PatientDto>>;

public sealed class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, PagedResult<PatientDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPatientsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PatientDto>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Patients.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(search) ||
                p.LastName.ToLower().Contains(search) ||
                (p.Phone != null && p.Phone.Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PatientDto(
                p.Id,
                p.FirstName,
                p.LastName,
                p.FirstName + " " + p.LastName,
                p.Email,
                p.Phone,
                p.DateOfBirth,
                p.Gender,
                p.Address,
                p.City,
                p.EmergencyContactName,
                p.EmergencyContactPhone,
                p.MedicalHistory,
                p.Allergies,
                p.Notes,
                p.IsActive,
                p.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<PatientDto>(items, totalCount, request.Page, request.PageSize);
    }
}
