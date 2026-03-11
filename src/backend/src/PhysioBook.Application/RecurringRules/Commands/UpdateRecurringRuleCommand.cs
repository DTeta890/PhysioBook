using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.RecurringRules.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.RecurringRules.Commands;

public sealed record UpdateRecurringRuleCommand(
    Guid Id,
    Guid TherapistId,
    Guid? PatientId,
    string? PatientName,
    string? PatientPhone,
    Guid TreatmentTypeId,
    string Frequency,
    int DayOfWeek,
    string StartTimeOfDay,
    string EndTimeOfDay,
    DateTimeOffset StartsFrom,
    DateTimeOffset? EndsAt,
    int? MaxOccurrences,
    string? Notes,
    string? Color,
    bool IsActive) : IRequest<RecurringRuleDto>;

public sealed class UpdateRecurringRuleCommandHandler : IRequestHandler<UpdateRecurringRuleCommand, RecurringRuleDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateRecurringRuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RecurringRuleDto> Handle(UpdateRecurringRuleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.RecurringRules
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(RecurringRule), request.Id);

        var therapist = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.TherapistId && u.IsTherapist, cancellationToken);

        if (therapist is null)
            throw new NotFoundException(nameof(User), request.TherapistId);

        var treatmentType = await _context.TreatmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TreatmentTypeId && t.IsActive, cancellationToken);

        if (treatmentType is null)
            throw new NotFoundException(nameof(TreatmentType), request.TreatmentTypeId);

        var startTime = TimeOnly.Parse(request.StartTimeOfDay);
        var endTime = TimeOnly.Parse(request.EndTimeOfDay);

        entity.TherapistId = request.TherapistId;
        entity.PatientId = request.PatientId;
        entity.TreatmentTypeId = request.TreatmentTypeId;
        entity.PatientName = request.PatientName;
        entity.PatientPhone = request.PatientPhone;
        entity.Frequency = request.Frequency;
        entity.DayOfWeek = request.DayOfWeek;
        entity.StartTimeOfDay = startTime;
        entity.EndTimeOfDay = endTime;
        entity.StartsFrom = request.StartsFrom;
        entity.EndsAt = request.EndsAt;
        entity.MaxOccurrences = request.MaxOccurrences;
        entity.Notes = request.Notes;
        entity.Color = request.Color;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return new RecurringRuleDto(
            entity.Id,
            entity.TherapistId,
            therapist.FullName,
            entity.PatientId,
            entity.PatientName,
            entity.PatientPhone,
            entity.TreatmentTypeId,
            treatmentType.Name,
            entity.Frequency,
            entity.DayOfWeek,
            entity.StartTimeOfDay.ToString("HH:mm"),
            entity.EndTimeOfDay.ToString("HH:mm"),
            entity.StartsFrom,
            entity.EndsAt,
            entity.MaxOccurrences,
            entity.Notes,
            entity.Color,
            entity.IsActive,
            entity.CreatedAt);
    }
}
