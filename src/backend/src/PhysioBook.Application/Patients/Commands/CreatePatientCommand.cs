using MediatR;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Patients.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Patients.Commands;

public sealed record CreatePatientCommand(
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
    string? Notes) : IRequest<PatientDto>;

public sealed class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto>
{
    private readonly IApplicationDbContext _context;

    public CreatePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientDto> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var entity = new Patient
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Address = request.Address,
            City = request.City,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            MedicalHistory = request.MedicalHistory,
            Allergies = request.Allergies,
            Notes = request.Notes,
        };

        _context.Patients.Add(entity);
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
