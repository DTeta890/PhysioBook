using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Appointments.DTOs;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.RecurringRules.Commands;

public sealed record GenerateAppointmentsCommand(
    Guid RecurringRuleId,
    DateTimeOffset FromDate,
    DateTimeOffset ToDate) : IRequest<List<AppointmentDto>>;

public sealed class GenerateAppointmentsCommandHandler : IRequestHandler<GenerateAppointmentsCommand, List<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICalendarNotificationService _notificationService;
    private readonly ICurrentTenantService _tenantService;

    public GenerateAppointmentsCommandHandler(
        IApplicationDbContext context,
        ICalendarNotificationService notificationService,
        ICurrentTenantService tenantService)
    {
        _context = context;
        _notificationService = notificationService;
        _tenantService = tenantService;
    }

    public async Task<List<AppointmentDto>> Handle(GenerateAppointmentsCommand request, CancellationToken cancellationToken)
    {
        var rule = await _context.RecurringRules
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RecurringRuleId, cancellationToken);

        if (rule is null)
            throw new NotFoundException(nameof(RecurringRule), request.RecurringRuleId);

        if (!rule.IsActive)
            throw new ConflictException("Recurring rule is not active.");

        var therapist = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == rule.TherapistId, cancellationToken);

        if (therapist is null)
            throw new NotFoundException(nameof(User), rule.TherapistId);

        var treatmentType = await _context.TreatmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == rule.TreatmentTypeId, cancellationToken);

        if (treatmentType is null)
            throw new NotFoundException(nameof(TreatmentType), rule.TreatmentTypeId);

        // Count existing appointments for this rule (for MaxOccurrences check)
        var existingCount = await _context.Appointments
            .CountAsync(a => a.RecurringRuleId == rule.Id, cancellationToken);

        // Get existing appointments from this rule in the date range to avoid duplicates
        var fromDateOnly = DateOnly.FromDateTime(request.FromDate.UtcDateTime);
        var toDateOnly = DateOnly.FromDateTime(request.ToDate.UtcDateTime);

        var existingAppointmentDates = await _context.Appointments
            .Where(a => a.RecurringRuleId == rule.Id
                && a.StartTime >= request.FromDate
                && a.StartTime <= request.ToDate)
            .Select(a => a.StartTime.Date)
            .ToListAsync(cancellationToken);

        var existingDatesSet = new HashSet<DateTime>(existingAppointmentDates);

        // Get all therapist appointments in the date range for conflict checking
        var therapistAppointments = await _context.Appointments
            .Where(a => a.TherapistId == rule.TherapistId
                && a.StartTime >= request.FromDate
                && a.EndTime <= request.ToDate.AddDays(1)
                && a.Status != "cancelled")
            .Select(a => new { a.StartTime, a.EndTime })
            .ToListAsync(cancellationToken);

        var startsFromDate = DateOnly.FromDateTime(rule.StartsFrom.UtcDateTime);
        var endsAtDate = rule.EndsAt.HasValue
            ? DateOnly.FromDateTime(rule.EndsAt.Value.UtcDateTime)
            : (DateOnly?)null;

        var createdAppointments = new List<AppointmentDto>();
        var currentDate = fromDateOnly;

        while (currentDate <= toDateOnly)
        {
            // Check MaxOccurrences
            if (rule.MaxOccurrences.HasValue && (existingCount + createdAppointments.Count) >= rule.MaxOccurrences.Value)
                break;

            // Check if date is within rule's effective range
            if (currentDate < startsFromDate)
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            if (endsAtDate.HasValue && currentDate > endsAtDate.Value)
                break;

            // Check if day of week matches
            if ((int)currentDate.DayOfWeek != rule.DayOfWeek)
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            // Check frequency pattern
            if (!MatchesFrequency(rule.Frequency, currentDate, startsFromDate))
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            // Check if appointment already exists for this date from this rule
            var currentDateTime = currentDate.ToDateTime(TimeOnly.MinValue);
            if (existingDatesSet.Contains(currentDateTime))
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            // Build the appointment times using UTC offset from the rule's StartsFrom
            var offset = rule.StartsFrom.Offset;
            var startDateTime = new DateTimeOffset(
                currentDate.Year, currentDate.Month, currentDate.Day,
                rule.StartTimeOfDay.Hour, rule.StartTimeOfDay.Minute, 0, offset);
            var endDateTime = new DateTimeOffset(
                currentDate.Year, currentDate.Month, currentDate.Day,
                rule.EndTimeOfDay.Hour, rule.EndTimeOfDay.Minute, 0, offset);

            // Check for time conflicts with existing therapist appointments
            var hasConflict = therapistAppointments.Any(a =>
                startDateTime < a.EndTime && endDateTime > a.StartTime);

            if (hasConflict)
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            // Also check against appointments we are creating in this batch
            var hasBatchConflict = createdAppointments.Any(a =>
                startDateTime < a.EndTime && endDateTime > a.StartTime);

            if (hasBatchConflict)
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            var appointment = new Appointment
            {
                TherapistId = rule.TherapistId,
                PatientId = rule.PatientId,
                TreatmentTypeId = rule.TreatmentTypeId,
                PatientName = rule.PatientName,
                PatientPhone = rule.PatientPhone,
                StartTime = startDateTime,
                EndTime = endDateTime,
                Status = "scheduled",
                Notes = rule.Notes,
                Color = rule.Color,
                IsWalkIn = false,
                RecurringRuleId = rule.Id,
            };

            _context.Appointments.Add(appointment);

            var dto = new AppointmentDto(
                appointment.Id,
                appointment.TherapistId,
                therapist.FullName,
                appointment.PatientId,
                appointment.PatientName,
                appointment.PatientPhone,
                appointment.TreatmentTypeId,
                treatmentType.Name,
                appointment.StartTime,
                appointment.EndTime,
                appointment.Status,
                appointment.Notes,
                appointment.CancellationReason,
                appointment.IsWalkIn,
                appointment.Color,
                appointment.RecurringRuleId,
                appointment.CreatedAt);

            createdAppointments.Add(dto);
            currentDate = currentDate.AddDays(1);
        }

        if (createdAppointments.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _notificationService.NotifyAppointmentsGenerated(_tenantService.TenantId, createdAppointments, cancellationToken);
        }

        return createdAppointments;
    }

    private static bool MatchesFrequency(string frequency, DateOnly currentDate, DateOnly startsFromDate)
    {
        return frequency switch
        {
            "weekly" => true, // day of week already checked
            "biweekly" => IsEvenWeekFromStart(currentDate, startsFromDate),
            "monthly" => IsFirstOccurrenceOfDayInMonth(currentDate),
            _ => false,
        };
    }

    private static bool IsEvenWeekFromStart(DateOnly currentDate, DateOnly startsFromDate)
    {
        var daysDiff = currentDate.DayNumber - startsFromDate.DayNumber;
        var weeksDiff = daysDiff / 7;
        return weeksDiff % 2 == 0;
    }

    private static bool IsFirstOccurrenceOfDayInMonth(DateOnly currentDate)
    {
        // The first occurrence of a given day-of-week in a month is always day 1-7
        return currentDate.Day <= 7;
    }
}
