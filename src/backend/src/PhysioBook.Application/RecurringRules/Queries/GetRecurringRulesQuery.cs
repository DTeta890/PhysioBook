using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.RecurringRules.DTOs;

namespace PhysioBook.Application.RecurringRules.Queries;

public sealed record GetRecurringRulesQuery(Guid? TherapistId = null) : IRequest<List<RecurringRuleDto>>;

public sealed class GetRecurringRulesQueryHandler : IRequestHandler<GetRecurringRulesQuery, List<RecurringRuleDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRecurringRulesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RecurringRuleDto>> Handle(GetRecurringRulesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.RecurringRules
            .AsNoTracking()
            .Include(r => r.Therapist)
            .Include(r => r.TreatmentType)
            .AsQueryable();

        if (request.TherapistId.HasValue)
        {
            query = query.Where(r => r.TherapistId == request.TherapistId.Value);
        }

        var entities = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(r => new RecurringRuleDto(
            r.Id,
            r.TherapistId,
            r.Therapist.FullName,
            r.PatientId,
            r.PatientName,
            r.PatientPhone,
            r.TreatmentTypeId,
            r.TreatmentType.Name,
            r.Frequency,
            r.DayOfWeek,
            r.StartTimeOfDay.ToString("HH:mm"),
            r.EndTimeOfDay.ToString("HH:mm"),
            r.StartsFrom,
            r.EndsAt,
            r.MaxOccurrences,
            r.Notes,
            r.Color,
            r.IsActive,
            r.CreatedAt)).ToList();
    }
}
