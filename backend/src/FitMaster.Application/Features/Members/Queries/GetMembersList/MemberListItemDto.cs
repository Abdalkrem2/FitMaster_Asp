namespace FitMaster.Application.Features.Members.Queries.GetMembersList;

public record MemberListItemDto(
    long MemberId,
    string Phone,
    string FullName,
    string? Gender,
    string? AddedByName,
    decimal? Debt,
    DateOnly? EndDate);
