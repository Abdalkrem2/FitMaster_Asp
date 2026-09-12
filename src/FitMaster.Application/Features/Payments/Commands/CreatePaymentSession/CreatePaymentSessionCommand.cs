using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Payments.Commands.CreatePaymentSession;

public record CheckoutSessionDto(long PaymentId, string CheckoutUrl);

/// <summary>Self-service: MemberId is always overwritten from the JWT in the
/// controller, never trusted from the request body (same pattern as
/// UpdateMemberProfileCommand's "me" endpoint).</summary>
public record CreatePaymentSessionCommand(
    long MemberId,
    long PackageId,
    string SuccessUrl,
    string CancelUrl) : IRequest<Result<CheckoutSessionDto>>;
