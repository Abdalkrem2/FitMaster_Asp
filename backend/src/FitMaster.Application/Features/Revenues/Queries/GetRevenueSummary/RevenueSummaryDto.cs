namespace FitMaster.Application.Features.Revenues.Queries.GetRevenueSummary;

public record RevenueSummaryDto(decimal TotalAmount, int TotalSales, List<RevenueByPackageDto> ByPackage);

public record RevenueByPackageDto(string PackageName, int Count, decimal Amount);
