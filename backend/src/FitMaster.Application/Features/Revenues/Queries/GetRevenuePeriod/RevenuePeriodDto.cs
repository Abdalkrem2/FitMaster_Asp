namespace FitMaster.Application.Features.Revenues.Queries.GetRevenuePeriod;

public record RevenuePeriodDto(decimal PeriodTotal, decimal PeriodDebt, List<RevenueRowDto> Revenues);

public record RevenueRowDto(
    long Id,
    string AddedByName,
    string MemberName,
    decimal Amount,
    string Pkg,
    decimal? Debt,
    string? Description,
    DateOnly CreatedAt);
