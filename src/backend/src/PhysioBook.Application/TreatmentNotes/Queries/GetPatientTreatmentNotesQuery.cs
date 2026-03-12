using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentNotes.DTOs;

namespace PhysioBook.Application.TreatmentNotes.Queries;

public sealed record GetPatientTreatmentNotesQuery(
    Guid PatientId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<TreatmentNoteDto>>;

public sealed class GetPatientTreatmentNotesQueryHandler : IRequestHandler<GetPatientTreatmentNotesQuery, PagedResult<TreatmentNoteDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPatientTreatmentNotesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<TreatmentNoteDto>> Handle(GetPatientTreatmentNotesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TreatmentNotes
            .AsNoTracking()
            .Include(t => t.Appointment)
            .Include(t => t.Patient)
            .Include(t => t.Therapist)
            .Where(t => t.PatientId == request.PatientId)
            .OrderByDescending(t => t.Appointment.StartTime);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(note => new TreatmentNoteDto(
                note.Id,
                note.AppointmentId,
                note.PatientId,
                note.Patient != null ? note.Patient.FirstName + " " + note.Patient.LastName : note.Appointment.PatientName,
                note.TherapistId,
                note.Therapist.FirstName + " " + note.Therapist.LastName,
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
                note.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<TreatmentNoteDto>(items, totalCount, request.Page, request.PageSize);
    }
}
