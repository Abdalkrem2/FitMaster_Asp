using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    difficulty_level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_archived = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "media_assets",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    license_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    attribution_text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media_assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "movement_patterns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movement_patterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "muscles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    body_region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_muscles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    reference_id = table.Column<long>(type: "bigint", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "packages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    duration_days = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_packages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    profile_picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    deleted = table.Column<bool>(type: "bit", nullable: false),
                    is_activated = table.Column<bool>(type: "bit", nullable: false),
                    admin_role_assigned_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exercise_equipment",
                columns: table => new
                {
                    exercise_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    equipment_id = table.Column<long>(type: "bigint", nullable: false),
                    is_required = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_equipment", x => new { x.exercise_id, x.equipment_id });
                    table.ForeignKey(
                        name: "FK_exercise_equipment_equipment_equipment_id",
                        column: x => x.equipment_id,
                        principalTable: "equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_exercise_equipment_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exercise_translations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    exercise_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    audio_cue_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Locale = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exercise_translations_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exercise_media",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    exercise_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    media_asset_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    view_angle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exercise_media_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercise_media_media_assets_media_asset_id",
                        column: x => x.media_asset_id,
                        principalTable: "media_assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exercise_patterns",
                columns: table => new
                {
                    exercise_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    pattern_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_patterns", x => new { x.exercise_id, x.pattern_id });
                    table.ForeignKey(
                        name: "FK_exercise_patterns_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercise_patterns_movement_patterns_pattern_id",
                        column: x => x.pattern_id,
                        principalTable: "movement_patterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exercise_muscles",
                columns: table => new
                {
                    exercise_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    muscle_id = table.Column<long>(type: "bigint", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_muscles", x => new { x.exercise_id, x.muscle_id });
                    table.ForeignKey(
                        name: "FK_exercise_muscles_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercise_muscles_muscles_muscle_id",
                        column: x => x.muscle_id,
                        principalTable: "muscles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exercise_tags",
                columns: table => new
                {
                    exercise_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    tag_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_tags", x => new { x.exercise_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_exercise_tags_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercise_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "activity_logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    performed_by = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    entity_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    entity_id = table.Column<long>(type: "bigint", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activity_logs_users_performed_by",
                        column: x => x.performed_by,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "member_profiles",
                columns: table => new
                {
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FitnessLevel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    split_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    training_style = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    injuries = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: true),
                    Height = table.Column<double>(type: "float", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    has_diabetes = table.Column<bool>(type: "bit", nullable: false),
                    has_heart_conditions = table.Column<bool>(type: "bit", nullable: false),
                    has_hypertension = table.Column<bool>(type: "bit", nullable: false),
                    allergies = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_profiles", x => x.member_id);
                    table.ForeignKey(
                        name: "FK_member_profiles_users_member_id",
                        column: x => x.member_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "memberships",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    package_id = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Debt = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_memberships_packages_package_id",
                        column: x => x.package_id,
                        principalTable: "packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_memberships_users_member_id",
                        column: x => x.member_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification_user_state",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    notification_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_user_state", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notification_user_state_notifications_notification_id",
                        column: x => x.notification_id,
                        principalTable: "notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notification_user_state_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nutrition_plans",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    daily_calories = table.Column<int>(type: "int", nullable: false),
                    protein_grams = table.Column<int>(type: "int", nullable: false),
                    carbs_grams = table.Column<int>(type: "int", nullable: false),
                    fat_grams = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nutrition_plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nutrition_plans_users_member_id",
                        column: x => x.member_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    RolesId = table.Column<long>(type: "bigint", nullable: false),
                    UsersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_plans",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Goal = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    split_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workout_plans_users_member_id",
                        column: x => x.member_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "revenues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    membership_id = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateOnly>(type: "date", nullable: false),
                    updated_at = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revenues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_revenues_memberships_membership_id",
                        column: x => x.membership_id,
                        principalTable: "memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_revenues_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_revenues_users_member_id",
                        column: x => x.member_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nutrition_meals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    meal_time = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    prep_time = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    total_calories = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nutrition_meals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nutrition_meals_nutrition_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "nutrition_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_days",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    workout_plan_id = table.Column<long>(type: "bigint", nullable: false),
                    muscle_group_label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    day_number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_days", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workout_days_workout_plans_workout_plan_id",
                        column: x => x.workout_plan_id,
                        principalTable: "workout_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "nutrition_foods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    meal_id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    protein_grams = table.Column<int>(type: "int", nullable: false),
                    carbs_grams = table.Column<int>(type: "int", nullable: false),
                    fat_grams = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nutrition_foods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nutrition_foods_nutrition_meals_meal_id",
                        column: x => x.meal_id,
                        principalTable: "nutrition_meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "nutrition_recipe_steps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    meal_id = table.Column<long>(type: "bigint", nullable: false),
                    step_order = table.Column<int>(type: "int", nullable: false),
                    Instruction = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nutrition_recipe_steps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nutrition_recipe_steps_nutrition_meals_meal_id",
                        column: x => x.meal_id,
                        principalTable: "nutrition_meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_exercises",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    workout_day_id = table.Column<long>(type: "bigint", nullable: false),
                    exercise_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: false),
                    Sets = table.Column<int>(type: "int", nullable: true),
                    Reps = table.Column<int>(type: "int", nullable: true),
                    reps_max = table.Column<int>(type: "int", nullable: true),
                    duration_seconds = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workout_exercises_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_workout_exercises_workout_days_workout_day_id",
                        column: x => x.workout_day_id,
                        principalTable: "workout_days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_performed_by",
                table: "activity_logs",
                column: "performed_by");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_Name",
                table: "equipment",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_exercise_equipment_equipment_id",
                table: "exercise_equipment",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_media_exercise_id",
                table: "exercise_media",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_media_media_asset_id",
                table: "exercise_media",
                column: "media_asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_muscles_muscle_id",
                table: "exercise_muscles",
                column: "muscle_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_patterns_pattern_id",
                table: "exercise_patterns",
                column: "pattern_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_tags_tag_id",
                table: "exercise_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_translations_exercise_id",
                table: "exercise_translations",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercises_Code",
                table: "exercises",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_memberships_member_id",
                table: "memberships",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_memberships_package_id",
                table: "memberships",
                column: "package_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_user_state_notification_id_user_id",
                table: "notification_user_state",
                columns: new[] { "notification_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_user_state_user_id",
                table: "notification_user_state",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_nutrition_foods_meal_id",
                table: "nutrition_foods",
                column: "meal_id");

            migrationBuilder.CreateIndex(
                name: "IX_nutrition_meals_plan_id",
                table: "nutrition_meals",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_nutrition_plans_member_id",
                table: "nutrition_plans",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_nutrition_recipe_steps_meal_id",
                table: "nutrition_recipe_steps",
                column: "meal_id");

            migrationBuilder.CreateIndex(
                name: "IX_revenues_created_by",
                table: "revenues",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_revenues_member_id",
                table: "revenues",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_revenues_membership_id",
                table: "revenues",
                column: "membership_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_UsersId",
                table: "user_roles",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_users_CreatedById",
                table: "users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_users_Phone",
                table: "users",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workout_days_workout_plan_id",
                table: "workout_days",
                column: "workout_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_exercises_exercise_id",
                table: "workout_exercises",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_exercises_workout_day_id",
                table: "workout_exercises",
                column: "workout_day_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_plans_member_id",
                table: "workout_plans",
                column: "member_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_logs");

            migrationBuilder.DropTable(
                name: "exercise_equipment");

            migrationBuilder.DropTable(
                name: "exercise_media");

            migrationBuilder.DropTable(
                name: "exercise_muscles");

            migrationBuilder.DropTable(
                name: "exercise_patterns");

            migrationBuilder.DropTable(
                name: "exercise_tags");

            migrationBuilder.DropTable(
                name: "exercise_translations");

            migrationBuilder.DropTable(
                name: "member_profiles");

            migrationBuilder.DropTable(
                name: "notification_user_state");

            migrationBuilder.DropTable(
                name: "nutrition_foods");

            migrationBuilder.DropTable(
                name: "nutrition_recipe_steps");

            migrationBuilder.DropTable(
                name: "revenues");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "workout_exercises");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "media_assets");

            migrationBuilder.DropTable(
                name: "muscles");

            migrationBuilder.DropTable(
                name: "movement_patterns");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "nutrition_meals");

            migrationBuilder.DropTable(
                name: "memberships");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "workout_days");

            migrationBuilder.DropTable(
                name: "nutrition_plans");

            migrationBuilder.DropTable(
                name: "packages");

            migrationBuilder.DropTable(
                name: "workout_plans");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
