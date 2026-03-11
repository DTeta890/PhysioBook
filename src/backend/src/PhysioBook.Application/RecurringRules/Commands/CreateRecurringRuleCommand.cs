using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.RecurringRules.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.RecurringRules.Commands;

public sealed record CreateRecurringRuleCommand(
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
    string? Color) : IRequest<RecurringRuleDto>;

public sealed class CreateRecurringRuleCommandHandler : IRequestHandler<CreateRecurringRuleCommand, RecurringRuleDto>
{
    private readonly IApplicationDbContext _context;

    public CreateRecurringRuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RecurringRuleDto> Handle(CreateRecurringRuleCommand request, CancellationToken cancellationToken)
    {
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

        var entity = new RecurringRule
        {
            TherapistId = request.TherapistId,
            PatientId = request.PatientId,
            TreatmentTypeId = request.TreatmentTypeId,
            PatientName = request.PatientName,
            PatientPhone = request.PatientPhone,
            Frequency = request.Frequency,
            DayOfWeek = request.DayOfWeek,
            StartTimeOfDay = startTime,
            EndTimeOfDay = endTime,
            StartsFrom = request.StartsFrom,
            EndsAt = request.EndsAt,
            MaxOccurrences = request.MaxOccurrences,
            Notes = request.Notes,
            Color = request.Color,
            IsActive = true,
        };

        _context.RecurringRules.Add(entity);
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
