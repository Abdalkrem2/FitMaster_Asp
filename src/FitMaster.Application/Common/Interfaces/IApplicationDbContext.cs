using FitMaster.Domain.Entities.ActivityLogging;
using FitMaster.Domain.Entities.Exercises;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Entities.Notifications;
using FitMaster.Domain.Entities.Nutrition;
using FitMaster.Domain.Entities.Workouts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;

namespace FitMaster.Application.Common.Interfaces;

/// <summary>
/// The persistence contract the Application layer programs against. Implemented
/// by FitMasterDbContext in Infrastructure. Handlers depend on this interface,
/// never on EF Core or the concrete DbContext directly.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }//get ????? ?? ??????? ??? DbSet ????? ???? ?? ????? ?? ?????/?????/??? ???????? ???? ??? DbSet.
    DbSet<Role> Roles { get; }

    DbSet<MemberProfile> MemberProfiles { get; }

    DbSet<Package> Packages { get; }
    DbSet<Membership> Memberships { get; }
    DbSet<Revenue> Revenues { get; }

    DbSet<Notification> Notifications { get; }
    DbSet<NotificationUserState> NotificationUserStates { get; }

    DbSet<ActivityLog> ActivityLogs { get; }

    DbSet<WorkoutPlan> WorkoutPlans { get; }
    DbSet<WorkoutDay> WorkoutDays { get; }
    DbSet<WorkoutExercise> WorkoutExercises { get; }

    DbSet<NutritionPlan> NutritionPlans { get; }
    DbSet<NutritionMeal> NutritionMeals { get; }
    DbSet<NutritionFood> NutritionFoods { get; }
    DbSet<NutritionRecipeStep> NutritionRecipeSteps { get; }

    // Reference data (read-only from the application's perspective)
    DbSet<Exercise> Exercises { get; }
    DbSet<Muscle> Muscles { get; }
    DbSet<Equipment> Equipment { get; }
    DbSet<Tag> Tags { get; }
    DbSet<MovementPattern> MovementPatterns { get; }
    DbSet<ExerciseTranslation> ExerciseTranslations { get; }
    DbSet<MediaAsset> MediaAssets { get; }
    DbSet<ExerciseMedia> ExerciseMedia { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    //We include SaveChangesAsync() in IApplicationDbContext so the Application layer can save
    //changes without depending directly on the concrete FitMasterDbContext.
}
