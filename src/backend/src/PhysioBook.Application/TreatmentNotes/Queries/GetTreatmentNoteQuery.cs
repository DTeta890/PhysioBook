using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentNotes.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentNotes.Queries;

public sealed record GetTreatmentNoteQuery(Guid Id) : IRequest<TreatmentNoteDto>;

public sealed class GetTreatmentNoteQueryHandler : IRequestHandler<GetTreatmentNoteQuery, TreatmentNoteDto>
{
    private readonly IApplicationDbContext _context;

    public GetTreatmentNoteQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TreatmentNoteDto> Handle(GetTreatmentNoteQuery request, CancellationToken cancellationToken)
    {
        var note = await _context.TreatmentNotes
            .AsNoTracking()
            .Include(t => t.Appointment)
            .Include(t => t.Patient)
            .Include(t => t.Therapist)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (note is null)
        {
            throw new NotFoundException(nameof(TreatmentNote), request.Id);
        }

        return new TreatmentNoteDto(
            note.Id,
            note.AppointmentId,
            note.PatientId,
            note.Patient?.FullName ?? note.Appointment.PatientName,
            note.TherapistId,
            note.Therapist.FullName,
            note.Appointment.StartTime,
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
