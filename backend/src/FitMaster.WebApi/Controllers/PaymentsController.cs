using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Payments.Commands.CreatePaymentSession;
using FitMaster.Application.Features.Payments.Commands.ProcessPaymentWebhook;
using FitMaster.Application.Features.Payments.Queries.GetMyPayments;
using FitMaster.Infrastructure.Payments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FitMaster.WebApi.Controllers;

public record CheckoutRequest(long PackageId);

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PaymentsController(
    ISender sender,
    ICurrentUserService currentUser,
    IConfiguration configuration,
    IOptions<StripeSettings> stripeSettings) : ControllerBase
{
    // Not a secret - exposed so the frontend could use Stripe.js/Elements later if it
    // ever moves off pure Checkout redirects. The current flow doesn't need it (the
    // frontend just follows the returned Checkout URL), but the config value exists
    // per spec, so it's exposed here too.
    [HttpGet("config")]
    [AllowAnonymous]
    public IActionResult GetConfig() => Ok(new { publishableKey = stripeSettings.Value.PublishableKey });

    [HttpGet("me")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await sender.Send(new GetMyPaymentsQuery(currentUser.UserId!.Value), ct));

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutRequest request, CancellationToken ct)
    {
        // Success/cancel URLs are built here from configured, trusted server-side
        // config - never from anything the client sends - so a checkout session can't
        // be used to redirect a member anywhere but this app's own frontend.
        var baseUrl = (configuration["Frontend:BaseUrl"] ?? "http://localhost:5173").TrimEnd('/');
        var successUrl = $"{baseUrl}/payments/success?session_id={{CHECKOUT_SESSION_ID}}";
        var cancelUrl = $"{baseUrl}/payments/cancel";

        var result = await sender.Send(
            new CreatePaymentSessionCommand(currentUser.UserId!.Value, request.PackageId, successUrl, cancelUrl), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    // Stripe calls this directly - it has no JWT, so it's explicitly excluded from
    // the controller's [Authorize]. The Stripe-Signature header (verified against
    // Stripe:WebhookSecret inside the handler) is what stands in for auth here -
    // without a valid signature the handler throws and this returns 400, never 200.
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(ct);
        var signature = Request.Headers["Stripe-Signature"].ToString();

        var result = await sender.Send(new ProcessPaymentWebhookCommand(payload, signature), ct);
        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }
}
