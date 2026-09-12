using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Payments;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Payments.Commands.CreatePaymentSession;

public class CreatePaymentSessionHandler(IApplicationDbContext db, IPaymentGatewayService gateway)
    : IRequestHandler<CreatePaymentSessionCommand, Result<CheckoutSessionDto>>
{
    public async Task<Result<CheckoutSessionDto>> Handle(CreatePaymentSessionCommand request, CancellationToken cancellationToken)
    {
        var memberExists = await db.Users.AnyAsync(u => u.Id == request.MemberId && !u.Deleted, cancellationToken);
        if (!memberExists)
        {
            return Result<CheckoutSessionDto>.Failure("Member not found.");
        }

        var package = await db.Packages
            .FirstOrDefaultAsync(p => p.Id == request.PackageId && !p.Deleted, cancellationToken);
        if (package is null)
        {
            return Result<CheckoutSessionDto>.Failure("Package not found.");
        }
        if (package.Status != PackageStatus.Active)
        {
            return Result<CheckoutSessionDto>.Failure("This package is not currently active and cannot be purchased.");
        }

        var payment = new Payment
        {
            MemberId = request.MemberId,
            PackageId = request.PackageId,
            Amount = package.Price,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };
        db.Payments.Add(payment);
        await db.SaveChangesAsync(cancellationToken);

        // The gateway session is created only after the Payment row exists, so its id
        // is available to round-trip as the checkout session's client-reference-id -
        // that's how the webhook correlates a confirmed payment back to this row.
        var session = await gateway.CreateCheckoutSessionAsync(
            new CheckoutSessionRequest(payment.Id, package.Name, package.Price, CustomerEmail: null, request.SuccessUrl, request.CancelUrl),
            cancellationToken);

        payment.GatewayReference = session.SessionId;
        await db.SaveChangesAsync(cancellationToken);

        return Result<CheckoutSessionDto>.Success(new CheckoutSessionDto(payment.Id, session.CheckoutUrl));
    }
}
