using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Infrastructure.Payments;
using Microsoft.Extensions.Options;
using Stripe;

namespace FitMaster.Application.Tests.Payments;

public class StripePaymentGatewayServiceTests
{
    private const string WebhookSecret = "whsec_test_secret_used_only_in_this_test";

    private static StripePaymentGatewayService CreateService()
        => new(Options.Create(new StripeSettings { SecretKey = "sk_test_unused_in_these_tests", WebhookSecret = WebhookSecret }));

    private static string BuildCheckoutSessionCompletedPayload(long paymentId, string sessionId = "cs_test_123", string paymentStatus = "paid")
        => $$"""
        {
          "id": "evt_test_123",
          "object": "event",
          "type": "checkout.session.completed",
          "data": {
            "object": {
              "id": "{{sessionId}}",
              "object": "checkout.session",
              "client_reference_id": "{{paymentId}}",
              "payment_status": "{{paymentStatus}}"
            }
          }
        }
        """;

    [Fact]
    public void ParseWebhookEvent_ValidSignature_CompletedSessionPaid_ReturnsCompletedWithPaymentId()
    {
        var payload = BuildCheckoutSessionCompletedPayload(paymentId: 42);
        var signatureHeader = EventUtility.GenerateSignatureHeader(payload, WebhookSecret, null);

        var result = CreateService().ParseWebhookEvent(payload, signatureHeader);

        Assert.Equal(GatewayPaymentOutcome.Completed, result.Outcome);
        Assert.Equal(42, result.PaymentId);
    }

    [Fact]
    public void ParseWebhookEvent_InvalidSignature_ThrowsInvalidWebhookSignatureException()
    {
        var payload = BuildCheckoutSessionCompletedPayload(paymentId: 42);
        var signatureHeader = EventUtility.GenerateSignatureHeader(payload, "whsec_a_completely_different_secret", null);

        Assert.Throws<InvalidWebhookSignatureException>(() => CreateService().ParseWebhookEvent(payload, signatureHeader));
    }

    [Fact]
    public void ParseWebhookEvent_TamperedPayload_ThrowsInvalidWebhookSignatureException()
    {
        var payload = BuildCheckoutSessionCompletedPayload(paymentId: 42);
        // Signature is computed over the ORIGINAL payload, then the payload actually
        // sent is altered afterwards - simulates someone replaying a captured header
        // against a forged body (e.g. changing which payment/amount gets completed).
        var signatureHeader = EventUtility.GenerateSignatureHeader(payload, WebhookSecret, null);
        var tamperedPayload = BuildCheckoutSessionCompletedPayload(paymentId: 999);

        Assert.Throws<InvalidWebhookSignatureException>(() => CreateService().ParseWebhookEvent(tamperedPayload, signatureHeader));
    }

    [Fact]
    public void ParseWebhookEvent_SessionCompletedButUnpaid_ReturnsIgnored()
    {
        // An async payment method (e.g. a bank debit) can complete the *session*
        // before the payment itself actually clears - must not provision a membership
        // yet; wait for async_payment_succeeded instead.
        var payload = BuildCheckoutSessionCompletedPayload(paymentId: 42, paymentStatus: "unpaid");
        var signatureHeader = EventUtility.GenerateSignatureHeader(payload, WebhookSecret, null);

        var result = CreateService().ParseWebhookEvent(payload, signatureHeader);

        Assert.Equal(GatewayPaymentOutcome.Ignored, result.Outcome);
    }

    [Fact]
    public void ParseWebhookEvent_SessionExpired_ReturnsCancelled()
    {
        var payload = $$"""
        {
          "id": "evt_test_456",
          "object": "event",
          "type": "checkout.session.expired",
          "data": {
            "object": {
              "id": "cs_test_456",
              "object": "checkout.session",
              "client_reference_id": "7",
              "payment_status": "unpaid"
            }
          }
        }
        """;
        var signatureHeader = EventUtility.GenerateSignatureHeader(payload, WebhookSecret, null);

        var result = CreateService().ParseWebhookEvent(payload, signatureHeader);

        Assert.Equal(GatewayPaymentOutcome.Cancelled, result.Outcome);
        Assert.Equal(7, result.PaymentId);
    }

    [Fact]
    public void ParseWebhookEvent_UnhandledEventType_ReturnsIgnored()
    {
        var payload = """
        {
          "id": "evt_test_789",
          "object": "event",
          "type": "customer.created",
          "data": { "object": { "id": "cus_test_789", "object": "customer" } }
        }
        """;
        var signatureHeader = EventUtility.GenerateSignatureHeader(payload, WebhookSecret, null);

        var result = CreateService().ParseWebhookEvent(payload, signatureHeader);

        Assert.Equal(GatewayPaymentOutcome.Ignored, result.Outcome);
        Assert.Null(result.PaymentId);
    }
}
