namespace FitMaster.Application.Features.Revenues.Queries.GetRevenueList;

public record RevenueListItemDto(
    long Id,
    long MemberId,
    string MemberFullName,
    long MembershipId,
    string PackageName,
    decimal Amount,
    string? Description,
    DateOnly CreatedAt);
