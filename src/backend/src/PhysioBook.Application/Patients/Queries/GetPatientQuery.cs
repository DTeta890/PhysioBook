using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Patients.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Patients.Queries;

public sealed record GetPatientQuery(Guid Id) : IRequest<PatientDto>;

public sealed class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, PatientDto>
{
    private readonly IApplicationDbContext _context;

    public GetPatientQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientDto> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Patient), request.Id);
        }

        return new PatientDto(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.FullName,
            entity.Email,
            entity.Phone,
            entity.DateOfBirth,
            entity.Gender,
            entity.Address,
            entity.City,
            entity.EmergencyContactName,
            entity.EmergencyContactPhone,
            entity.MedicalHistory,
            entity.Allergies,
            entity.Notes,
            entity.IsActive,
            entity.CreatedAt);
    }
}
