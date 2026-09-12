using FitMaster.Domain.Entities.Nutrition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitMaster.Infrastructure.Persistence.Configurations;

public class NutritionPlanConfiguration : IEntityTypeConfiguration<NutritionPlan>
{
    public void Configure(EntityTypeBuilder<NutritionPlan> builder)
    {
        builder.ToTable("nutrition_plans");

        builder.Property(p => p.MemberId).HasColumnName("member_id").IsRequired();
        builder.HasOne(p => p.Member).WithMany(u => u.NutritionPlans)
            .HasForeignKey(p => p.MemberId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Goal).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(p => p.DailyCalories).HasColumnName("daily_calories").IsRequired();
        builder.Property(p => p.ProteinGrams).HasColumnName("protein_grams").IsRequired();
        builder.Property(p => p.CarbsGrams).HasColumnName("carbs_grams").IsRequired();
        builder.Property(p => p.FatGrams).HasColumnName("fat_grams").IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasMany(p => p.Meals).WithOne(m => m.NutritionPlan)
            .HasForeignKey(m => m.NutritionPlanId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class NutritionMealConfiguration : IEntityTypeConfiguration<NutritionMeal>
{
    public void Configure(EntityTypeBuilder<NutritionMeal> builder)
    {
        builder.ToTable("nutrition_meals");

        builder.Property(m => m.NutritionPlanId).HasColumnName("plan_id").IsRequired();
        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.MealTime).HasColumnName("meal_time").HasMaxLength(50).IsRequired();
        builder.Property(m => m.PrepTime).HasColumnName("prep_time").HasMaxLength(50);
        builder.Property(m => m.TotalCalories).HasColumnName("total_calories").IsRequired();

        builder.HasMany(m => m.Foods).WithOne(f => f.Meal)
            .HasForeignKey(f => f.MealId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(m => m.RecipeSteps).WithOne(s => s.Meal)
            .HasForeignKey(s => s.MealId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class NutritionFoodConfiguration : IEntityTypeConfiguration<NutritionFood>
{
    public void Configure(EntityTypeBuilder<NutritionFood> builder)
    {
        builder.ToTable("nutrition_foods");

        builder.Property(f => f.MealId).HasColumnName("meal_id").IsRequired();
        builder.Property(f => f.Name).HasMaxLength(200).IsRequired();
        builder.Property(f => f.Amount).HasMaxLength(50).IsRequired();
        builder.Property(f => f.Calories).IsRequired();
        builder.Property(f => f.ProteinGrams).HasColumnName("protein_grams").IsRequired();
        builder.Property(f => f.CarbsGrams).HasColumnName("carbs_grams").IsRequired();
        builder.Property(f => f.FatGrams).HasColumnName("fat_grams").IsRequired();
    }
}

public class NutritionRecipeStepConfiguration : IEntityTypeConfiguration<NutritionRecipeStep>
{
    public void Configure(EntityTypeBuilder<NutritionRecipeStep> builder)
    {
        builder.ToTable("nutrition_recipe_steps");

        builder.Property(s => s.MealId).HasColumnName("meal_id").IsRequired();
        builder.Property(s => s.StepOrder).HasColumnName("step_order").IsRequired();
        builder.Property(s => s.Instruction).IsRequired();
    }
}
