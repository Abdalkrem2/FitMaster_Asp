using FitMaster.Domain.Common;
using FitMaster.Domain.Entities.ActivityLogging;
using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Entities.Notifications;
using FitMaster.Domain.Entities.Nutrition;
using FitMaster.Domain.Entities.Workouts;

namespace FitMaster.Domain.Entities.Identity;

/// <summary>
/// Any person who can sign in: a member, an employee, or an admin.
/// Which capabilities they have is driven by the assigned <see cref="Roles"/>.
/// </summary>
public class User : BaseEntity
{
    public required string Phone { get; set; }

    public required string PasswordHash { get; set; }

    public required string FullName { get; set; }

    public string? ProfilePicture { get; set; }

    public string? Gender { get; set; }

    public bool Deleted { get; set; }

    public bool IsActivated { get; set; }

    /// <summary>When this user was granted the Admin role, if ever.</summary>
    public DateTime? AdminRoleAssignedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    /// <summary>The user (typically an admin/employee) who created this account.</summary>
    public long? CreatedById { get; set; }

    public User? CreatedBy { get; set; }

    public ICollection<Role> Roles { get; set; } = new List<Role>();

    // --- Reverse navigations -------------------------------------------------

    /// <summary>Present only when this user is a Member.</summary>
    public MemberProfile? MemberProfile { get; set; }

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();

    public ICollection<NutritionPlan> NutritionPlans { get; set; } = new List<NutritionPlan>();

    public ICollection<ActivityLog> ActivityLogsPerformed { get; set; } = new List<ActivityLog>();

    public ICollection<NotificationUserState> NotificationStates { get; set; } = new List<NotificationUserState>();
}
