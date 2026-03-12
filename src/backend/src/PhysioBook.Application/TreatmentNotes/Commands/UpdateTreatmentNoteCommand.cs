using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentNotes.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentNotes.Commands;

public sealed record UpdateTreatmentNoteCommand(
    Guid Id,
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

public sealed class UpdateTreatmentNoteCommandHandler : IRequestHandler<UpdateTreatmentNoteCommand, TreatmentNoteDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateTreatmentNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TreatmentNoteDto> Handle(UpdateTreatmentNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.TreatmentNotes
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (note is null)
        {
            throw new NotFoundException(nameof(TreatmentNote), request.Id);
        }

        note.Subjective = request.Subjective;
        note.Objective = request.Objective;
        note.Assessment = request.Assessment;
        note.Plan = request.Plan;
        note.Diagnosis = request.Diagnosis;
        note.TreatmentProvided = request.TreatmentProvided;
        note.PainLevelBefore = request.PainLevelBefore;
        note.PainLevelAfter = request.PainLevelAfter;
        note.RangeOfMotionNotes = request.RangeOfMotionNotes;
        note.ExercisesPrescribed = request.ExercisesPrescribed;
        note.FollowUpInstructions = request.FollowUpInstructions;

        await _context.SaveChangesAsync(cancellationToken);

        var appointment = await _context.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == note.AppointmentId, cancellationToken);

        var therapist = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == note.TherapistId, cancellationToken);

        string? patientName = appointment?.PatientName;
        if (note.PatientId.HasValue)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == note.PatientId.Value, cancellationToken);

            if (patient is not null)
            {
                patientName = patient.FullName;
            }
        }

        return new TreatmentNoteDto(
            note.Id,
            note.AppointmentId,
            note.PatientId,
            patientName,
            note.TherapistId,
            therapist?.FullName ?? string.Empty,
            appointment?.StartTime ?? default,
            note.Subjective,
            note.Objective,
            note.Assessment,
            note.Plan,
            note.Diagnosis,
            note.TreatmentProvided,
            note.PainLevelBefore,
            note.PainLevelAfter,
            note.RangeOfMotionNotes,
            note.ExercisesPrescribed,
            note.FollowUpInstructions,
            note.IsSigned,
            note.SignedAt,
            note.CreatedAt);
    }
}
