using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentNotes.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentNotes.Commands;

public sealed record CreateTreatmentNoteCommand(
    Guid AppointmentId,
    string? Subjective,
    string? Objective,
    string? Assessment,
    string? Plan,
    string? Diagnosis,
    string? TreatmentProvided,
    int? PainLevelBefore,
    int? PainLevelAfter,
    string? RangeOfMotionNotes,
    string? ExercisesPrescribed,
    string? FollowUpInstructions) : IRequest<TreatmentNoteDto>;

public sealed class CreateTreatmentNoteCommandHandler : IRequestHandler<CreateTreatmentNoteCommand, TreatmentNoteDto>
{
    private readonly IApplicationDbContext _context;

    public CreateTreatmentNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TreatmentNoteDto> Handle(CreateTreatmentNoteCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

        if (appointment is null)
        {
            throw new NotFoundException(nameof(Appointment), request.AppointmentId);
        }

        var existingNote = await _context.TreatmentNotes
            .AsNoTracking()
            .AnyAsync(t => t.AppointmentId == request.AppointmentId, cancellationToken);

        if (existingNote)
        {
            throw new ConflictException("A treatment note already exists for this appointment.");
        }

        var therapist = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == appointment.TherapistId, cancellationToken);

        if (therapist is null)
        {
            throw new NotFoundException("Therapist", appointment.TherapistId);
        }

        string? patientName = appointment.PatientName;
        if (appointment.PatientId.HasValue)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == appointment.PatientId.Value, cancellationToken);

            if (patient is not null)
            {
                patientName = patient.FullName;
            }
        }

        var entity = new TreatmentNote
        {
            AppointmentId = request.AppointmentId,
            PatientId = appointment.PatientId,
            TherapistId = appointment.TherapistId,
            Subjective = request.Subjective,
            Objective = request.Objective,
            Assessment = request.Assessment,
            Plan = request.Plan,
            Diagnosis = request.Diagnosis,
            TreatmentProvided = request.TreatmentProvided,
            PainLevelBefore = request.PainLevelBefore,
            PainLevelAfter = request.PainLevelAfter,
            RangeOfMotionNotes = request.RangeOfMotionNotes,
            ExercisesPrescribed = request.ExercisesPrescribed,
            FollowUpInstructions = request.FollowUpInstructions,
        };

        _context.TreatmentNotes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new TreatmentNoteDto(
            entity.Id,
            entity.AppointmentId,
            entity.PatientId,
            patientName,
            entity.TherapistId,
            therapist.FullName,
            appointment.StartTime,
            entity.Subjective,
            entity.Objective,
            entity.Assessment,
            entity.Plan,
            entity.Diagnosis,
            entity.TreatmentProvided,
            entity.PainLevelBefore,
            entity.PainLevelAfter,
            entity.RangeOfMotionNotes,
            entity.ExercisesPrescribed,
            entity.FollowUpInstructions,
            entity.IsSigned,
            entity.SignedAt,
            entity.CreatedAt);
    }
}
