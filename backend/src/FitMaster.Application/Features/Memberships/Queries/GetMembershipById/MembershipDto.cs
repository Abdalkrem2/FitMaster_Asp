using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Memberships.Queries.GetMembershipById;

public record MembershipDto(
    long Id,
    long MemberId,
    string MemberFullName,
    long PackageId,
    string PackageName,
    MembershipStatus Status,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Price,
    decimal? Debt,
    string? Description);
