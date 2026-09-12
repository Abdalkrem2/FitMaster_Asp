using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Enums;

namespace FitMaster.Domain.Entities.Payments;

/// <summary>
/// A single self-service online payment attempt for a membership package,
/// processed through a payment gateway (Stripe). Starts <see cref="PaymentStatus.Pending"/>
/// as soon as a checkout session is created; only the gateway's webhook (never
/// the frontend redirect) is allowed to move it to <see cref="PaymentStatus.Completed"/>,
/// at which point <see cref="MembershipId"/> is filled in with the Membership it produced.
/// </summary>
public class Payment : BaseEntity
{
    public required long MemberId { get; set; }

    public User Member { get; set; } = null!;

    public required long PackageId { get; set; }

    public Package Package { get; set; } = null!;

    public required decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>The gateway's checkout session id (e.g. Stripe's "cs_..."), used to
    /// correlate a webhook event back to this payment.</summary>
    public string? GatewayReference { get; set; }

    /// <summary>Set once the webhook confirms payment and provisions a Membership.</summary>
    public long? MembershipId { get; set; }

    public Membership? Membership { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}
