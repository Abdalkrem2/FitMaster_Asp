using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace FitMaster.Infrastructure.Payments;

/// <summary>
/// Stripe-backed implementation of IPaymentGatewayService, using Stripe Checkout
/// (a hosted payment page) rather than a custom Stripe Elements form. The Payment's
/// own id is round-tripped as the checkout session's client_reference_id, which is
/// how a later webhook event gets correlated back to the right Payment row without
/// trusting anything the client itself sends.
/// </summary>
public class StripePaymentGatewayService(IOptions<StripeSettings> settings) : IPaymentGatewayService
{
    private readonly StripeClient _client = new(settings.Value.SecretKey);

    public async Task<CheckoutSessionResult> CreateCheckoutSessionAsync(CheckoutSessionRequest request, CancellationToken cancellationToken)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)Math.Round(request.Amount * 100m, MidpointRounding.AwayFromZero),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = request.PackageName,
                        },
                    },
                },
            ],
            ClientReferenceId = request.PaymentId.ToString(),
            CustomerEmail = request.CustomerEmail,
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
        };

        var service = new SessionService(_client);
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return new CheckoutSessionResult(session.Id, session.Url);
    }

    public GatewayWebhookResult ParseWebhookEvent(string payload, string signatureHeader)
    {
        Event stripeEvent;
        try
        {
            // throwOnApiVersionMismatch: false - the payload is already
            // signature-verified by this point, so a Stripe API-version drift
            // between the event and this SDK build shouldn't hard-fail the whole
            // webhook; we only read a handful of stable fields off it below.
            stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, settings.Value.WebhookSecret, throwOnApiVersionMismatch: false);
        }
        catch (StripeException ex)
        {
            throw new InvalidWebhookSignatureException($"Stripe webhook signature verification failed: {ex.Message}");
        }

        return stripeEvent.Type switch
        {
            // "paid" is the normal card-payment case; "no_payment_required" covers a
            // (theoretical here) zero-amount session. Anything else means an async
            // payment method is still pending - wait for the async_payment_* event below.
            "checkout.session.completed" => ParseSessionEvent(stripeEvent,
                s => s.PaymentStatus is "paid" or "no_payment_required" ? GatewayPaymentOutcome.Completed : GatewayPaymentOutcome.Ignored),
            "checkout.session.async_payment_succeeded" => ParseSessionEvent(stripeEvent, _ => GatewayPaymentOutcome.Completed),
            "checkout.session.async_payment_failed" => ParseSessionEvent(stripeEvent, _ => GatewayPaymentOutcome.Failed),
            // Fires once the session's expiration window (24h by default) passes without
            // completing - the only signal we get for "the member just clicked away and
            // never paid", since a plain cancel-button click never reaches Stripe's servers.
            "checkout.session.expired" => ParseSessionEvent(stripeEvent, _ => GatewayPaymentOutcome.Cancelled),
            _ => new GatewayWebhookResult(stripeEvent.Id, GatewayPaymentOutcome.Ignored, null),
        };
    }

    private static GatewayWebhookResult ParseSessionEvent(Event stripeEvent, Func<Session, GatewayPaymentOutcome> resolveOutcome)
    {
        if (stripeEvent.Data.Object is not Session session)
        {
            return new GatewayWebhookResult(stripeEvent.Id, GatewayPaymentOutcome.Ignored, null);
        }

        if (!long.TryParse(session.ClientReferenceId, out var paymentId))
        {
            return new GatewayWebhookResult(stripeEvent.Id, GatewayPaymentOutcome.Ignored, null);
        }

        return new GatewayWebhookResult(stripeEvent.Id, resolveOutcome(session), paymentId);
    }
}
