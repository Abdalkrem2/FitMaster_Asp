using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Payments.Queries.GetMyPayments;

public record PaymentDto(
    long Id,
    long PackageId,
    string PackageName,
    decimal Amount,
    PaymentStatus Status,
    long? MembershipId,
    DateTime CreatedAt,
    DateTime? CompletedAt);
