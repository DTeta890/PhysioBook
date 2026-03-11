using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.RecurringRules.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.RecurringRules.Queries;

public sealed record GetRecurringRuleQuery(Guid Id) : IRequest<RecurringRuleDto>;

public sealed class GetRecurringRuleQueryHandler : IRequestHandler<GetRecurringRuleQuery, RecurringRuleDto>
{
    private readonly IApplicationDbContext _context;

    public GetRecurringRuleQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RecurringRuleDto> Handle(GetRecurringRuleQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.RecurringRules
            .AsNoTracking()
            .Include(r => r.Therapist)
            .Include(r => r.TreatmentType)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(RecurringRule), request.Id);

        return new RecurringRuleDto(
            entity.Id,
            entity.TherapistId,
            entity.Therapist.FullName,
            entity.PatientId,
            entity.PatientName,
            entity.PatientPhone,
            entity.TreatmentTypeId,
            entity.TreatmentType.Name,
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
