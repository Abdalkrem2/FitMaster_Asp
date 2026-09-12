using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Payments.Commands.ProcessPaymentWebhook;

/// <summary>Not a typical user-initiated command - Stripe calls this, not a logged-in
/// user, so there's no ICurrentUserService context. Modeled as a command anyway (rather
/// than a bespoke non-MediatR service) to stay consistent with how every other
/// write path in this app is shaped.</summary>
public record ProcessPaymentWebhookCommand(string Payload, string SignatureHeader) : IRequest<Result>;
