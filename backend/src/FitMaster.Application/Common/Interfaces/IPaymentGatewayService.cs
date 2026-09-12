namespace FitMaster.Application.Common.Interfaces;

public record CheckoutSessionRequest(
    long PaymentId,
    string PackageName,
    decimal Amount,
    string? CustomerEmail,
    string SuccessUrl,
    string CancelUrl);

public record CheckoutSessionResult(string SessionId, string CheckoutUrl);

public enum GatewayPaymentOutcome
{
    /// <summary>Payment confirmed - safe to provision the membership.</summary>
    Completed,

    /// <summary>Payment attempted but did not succeed.</summary>
    Failed,

    /// <summary>Checkout was abandoned/expired without ever completing.</summary>
    Cancelled,

    /// <summary>A legitimate, signature-verified event this app has no action for
    /// (e.g. an event type we don't subscribe to). Callers should acknowledge it
    /// (200) without touching any Payment.</summary>
    Ignored,
}

/// <summary>Gateway-agnostic result of parsing one verified webhook event. <see cref="PaymentId"/>
/// is our own Payment.Id (round-tripped via the checkout session's client-reference-id),
/// null when <see cref="Outcome"/> is <see cref="GatewayPaymentOutcome.Ignored"/>.</summary>
public record GatewayWebhookResult(string EventId, GatewayPaymentOutcome Outcome, long? PaymentId);

/// <summary>
/// Abstraction over the payment gateway (Stripe) - the Application layer creates
/// checkout sessions and parses webhook events through this interface, never
/// touching the Stripe SDK directly. Implemented in Infrastructure.
/// </summary>
public interface IPaymentGatewayService
{
    Task<CheckoutSessionResult> CreateCheckoutSessionAsync(CheckoutSessionRequest request, CancellationToken cancellationToken);

    /// <summary>Verifies <paramref name="signatureHeader"/> against the configured webhook
    /// secret and translates the event into a gateway-agnostic result. Throws
    /// InvalidWebhookSignatureException if the signature doesn't check out - this is
    /// what stops the webhook endpoint being usable as a backdoor by anyone who isn't
    /// actually Stripe.</summary>
    GatewayWebhookResult ParseWebhookEvent(string payload, string signatureHeader);
}
