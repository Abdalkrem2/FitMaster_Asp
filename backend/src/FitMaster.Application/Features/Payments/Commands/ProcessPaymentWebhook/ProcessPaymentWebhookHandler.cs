using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Application.MembershipProvisioning;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Payments.Commands.ProcessPaymentWebhook;

public class ProcessPaymentWebhookHandler(
    IApplicationDbContext db,
    IPaymentGatewayService gateway,
    IMembershipProvisioningService provisioning,
    IPublisher publisher) : IRequestHandler<ProcessPaymentWebhookCommand, Result>
{
    public async Task<Result> Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        // Throws InvalidWebhookSignatureException on a bad signature - lets that
        // propagate to GlobalExceptionHandler (-> HTTP 400), never caught here.
        var webhookResult = gateway.ParseWebhookEvent(request.Payload, request.SignatureHeader);

        if (webhookResult.Outcome == GatewayPaymentOutcome.Ignored || webhookResult.PaymentId is null)
        {
            // A legitimate, verified event this app doesn't act on - acknowledge it
            // (the controller returns 200) so Stripe doesn't keep retrying it.
            return Result.Success();
        }

        var payment = await db.Payments
            .Include(p => p.Member)
            .Include(p => p.Package)
            .FirstOrDefaultAsync(p => p.Id == webhookResult.PaymentId.Value, cancellationToken);
        if (payment is null)
        {
            return Result.Failure("Payment not found for this webhook event.");
        }

        // Idempotency: Stripe can and will redeliver the same event. Once a payment
        // has moved off Pending it's never touched again, regardless of how many more
        // times this event (or a resend of it) arrives - that's what stops a redelivered
        // "completed" event from creating a second Membership/Revenue row.
        if (payment.Status != PaymentStatus.Pending)
        {
            return Result.Success();
        }

        var now = DateTime.UtcNow;

        if (webhookResult.Outcome is GatewayPaymentOutcome.Failed or GatewayPaymentOutcome.Cancelled)
        {
            payment.Status = webhookResult.Outcome == GatewayPaymentOutcome.Failed
                ? PaymentStatus.Failed
                : PaymentStatus.Cancelled;
            payment.CompletedAt = now;
            await db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        // Completed - provision the membership through the exact same stacking logic
        // the staff-driven CreateMembershipHandler uses, so an online purchase behaves
        // identically to one a staff member enters manually.
        var membership = await provisioning.ProvisionAsync(
            payment.MemberId,
            payment.Package,
            DateOnly.FromDateTime(now),
            payment.Amount,
            payment.Amount, // paid in full through Stripe - no partial/debt concept here
            description: "Online payment via Stripe",
            revenueCreatedById: payment.MemberId, // self-service - the member's own action produced this revenue
            revenueDescription: $"Online payment - {payment.Package.Name}",
            cancellationToken);

        payment.Status = PaymentStatus.Completed;
        payment.CompletedAt = now;
        payment.MembershipId = membership.Id;
        await db.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new ActivityOccurredEvent(
            payment.MemberId, ActionType.Create, EntityType.Payment, payment.Id,
            $"Online payment received from \"{payment.Member.FullName}\" for \"{payment.Package.Name}\" (${payment.Amount:0.00})"),
            cancellationToken);

        return Result.Success();
    }
}
