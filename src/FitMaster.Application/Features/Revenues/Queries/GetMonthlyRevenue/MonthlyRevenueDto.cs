namespace FitMaster.Application.Features.Revenues.Queries.GetMonthlyRevenue;

public record MonthlyRevenueDto(decimal YearTotal, Dictionary<int, decimal> Months);
