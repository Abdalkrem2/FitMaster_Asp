using FitMaster.Application.Common.Interfaces;

namespace FitMaster.Application.Tests.Payments;

/// <summary>Test double for IPaymentGatewayService - lets handler-level tests control
/// exactly what "the gateway" returns without any real Stripe signature parsing
/// (that's covered separately by StripePaymentGatewayServiceTests).</summary>
internal sealed class FakePaymentGatewayService : IPaymentGatewayService
{
    public CheckoutSessionResult? SessionToReturn { get; set; }
        = new("cs_test_fake", "https://checkout.stripe.com/test");

    public GatewayWebhookResult? WebhookResultToReturn { get; set; }

    public Task<CheckoutSessionResult> CreateCheckoutSessionAsync(CheckoutSessionRequest request, CancellationToken cancellationToken)
        => Task.FromResult(SessionToReturn!);

    public GatewayWebhookResult ParseWebhookEvent(string payload, string signatureHeader)
        => WebhookResultToReturn!;
}
