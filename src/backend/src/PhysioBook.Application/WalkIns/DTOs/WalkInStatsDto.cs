namespace PhysioBook.Application.WalkIns.DTOs;

public sealed record WalkInStatsDto(
    int TotalWalkIns,
    double AverageWaitMinutes,
    int ServedCount,
    int NoShowCount,
    int CancelledCount,
    int? PeakHour);
