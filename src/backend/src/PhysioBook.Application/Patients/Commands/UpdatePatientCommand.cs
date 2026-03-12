using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Patients.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Patients.Commands;

public sealed record UpdatePatientCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    DateOnly? DateOfBirth,
    string? Gender,
    string? Address,
    string? City,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? MedicalHistory,
    string? Allergies,
    string? Notes,
    bool IsActive) : IRequest<PatientDto>;

public sealed class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, PatientDto>
{
    private readonly IApplicationDbContext _context;

    public UpdatePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientDto> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Patient), request.Id);
        }

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.DateOfBirth = request.DateOfBirth;
        entity.Gender = request.Gender;
        entity.Address = request.Address;
        entity.City = request.City;
        entity.EmergencyContactName = request.EmergencyContactName;
        entity.EmergencyContactPhone = request.EmergencyContactPhone;
        entity.MedicalHistory = request.MedicalHistory;
        entity.Allergies = request.Allergies;
        entity.Notes = request.Notes;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

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
