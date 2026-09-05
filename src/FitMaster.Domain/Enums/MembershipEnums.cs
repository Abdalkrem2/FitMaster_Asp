namespace FitMaster.Domain.Enums;

/// <summary>Lifecycle status of a member's Membership.</summary>
public enum MembershipStatus
{
    Active,
    Expired,
    Frozen,
    Canceled
}

/// <summary>Whether a subscription Package can currently be sold.</summary>
public enum PackageStatus
{
    Active,
    Inactive
}

/// <summary>How a payment was made.</summary>
public enum PaymentMethod
{
    Subscription,
    Cash,
    Card,
    Online
}

/// <summary>Categorizes an incoming Revenue record.</summary>
public enum RevenueType
{
    /// <summary>Monthly / yearly membership subscription.</summary>
    Subscription,

    /// <summary>Sale of a supplement or other physical product.</summary>
    Product,

    Other
}
