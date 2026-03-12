using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.WalkIns.DTOs;

namespace PhysioBook.Application.WalkIns.Queries;

public sealed record GetWalkInStatsQuery(
    DateTimeOffset From,
    DateTimeOffset To) : IRequest<WalkInStatsDto>;

public sealed class GetWalkInStatsQueryHandler : IRequestHandler<GetWalkInStatsQuery, WalkInStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetWalkInStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WalkInStatsDto> Handle(GetWalkInStatsQuery request, CancellationToken cancellationToken)
    {
        var entries = await _context.WalkInEntries
            .AsNoTracking()
            .Where(w => w.CheckedInAt >= request.From && w.CheckedInAt < request.To)
            .ToListAsync(cancellationToken);

        var totalWalkIns = entries.Count;
        var servedCount = entries.Count(w => w.Status == "served");
        var noShowCount = entries.Count(w => w.Status == "no_show");
        var cancelledCount = entries.Count(w => w.Status == "cancelled");

        var servedEntries = entries
            .Where(w => w.Status == "served" && w.CalledAt.HasValue)
            .ToList();

        var averageWaitMinutes = servedEntries.Count > 0
            ? servedEntries.Average(w => (w.CalledAt!.Value - w.CheckedInAt).TotalMinutes)
            : 0;

        int? peakHour = entries.Count > 0
            ? entries
                .GroupBy(w => w.CheckedInAt.Hour)
                .OrderByDescending(g => g.Count())
                .First().Key
            : null;

        return new WalkInStatsDto(
            totalWalkIns,
            Math.Round(averageWaitMinutes, 1),
            servedCount,
            noShowCount,
            cancelledCount,
            peakHour);
    }
}
