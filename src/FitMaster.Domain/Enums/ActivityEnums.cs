namespace FitMaster.Domain.Enums;

/// <summary>What a Notification is about. Currently only activity-log driven.</summary>
public enum NotificationType
{
    ActivityLog
}

/// <summary>The kind of action recorded in an ActivityLog entry.</summary>
public enum ActionType
{
    Create,
    Update,
    Delete,
    Renew,
    Login,
    Add
}

/// <summary>The kind of entity an ActivityLog entry refers to.</summary>
public enum EntityType
{
    Member,
    Employee,
    Membership
}
