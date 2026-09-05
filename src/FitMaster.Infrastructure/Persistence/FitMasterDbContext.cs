using System.Reflection;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.ActivityLogging;
using FitMaster.Domain.Entities.Exercises;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Entities.Notifications;
using FitMaster.Domain.Entities.Nutrition;
using FitMaster.Domain.Entities.Workouts;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Infrastructure.Persistence;


public class FitMasterDbContext(DbContextOptions<FitMasterDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    public DbSet<MemberProfile> MemberProfiles => Set<MemberProfile>();

    public DbSet<Package> Packages => Set<Package>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Revenue> Revenues => Set<Revenue>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationUserState> NotificationUserStates => Set<NotificationUserState>();

    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    public DbSet<WorkoutPlan> WorkoutPlans => Set<WorkoutPlan>();
    public DbSet<WorkoutDay> WorkoutDays => Set<WorkoutDay>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();

    public DbSet<NutritionPlan> NutritionPlans => Set<NutritionPlan>();
    public DbSet<NutritionMeal> NutritionMeals => Set<NutritionMeal>();
    public DbSet<NutritionFood> NutritionFoods => Set<NutritionFood>();
    public DbSet<NutritionRecipeStep> NutritionRecipeSteps => Set<NutritionRecipeStep>();

    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<Muscle> Muscles => Set<Muscle>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<MovementPattern> MovementPatterns => Set<MovementPattern>();
    public DbSet<ExerciseTranslation> ExerciseTranslations => Set<ExerciseTranslation>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<ExerciseMedia> ExerciseMedia => Set<ExerciseMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Picks up every IEntityTypeConfiguration<T> class in this assembly
        // (see Persistence/Configurations/*) instead of configuring everything
        // inline here.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
