using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInjuryMuscleExclusions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "injury_muscle_exclusions",
                columns: table => new
                {
                    injury_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    muscle_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_injury_muscle_exclusions", x => new { x.injury_type, x.muscle_id });
                    table.ForeignKey(
                        name: "FK_injury_muscle_exclusions_muscles_muscle_id",
                        column: x => x.muscle_id,
                        principalTable: "muscles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_injury_muscle_exclusions_muscle_id",
                table: "injury_muscle_exclusions",
                column: "muscle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "injury_muscle_exclusions");
        }
    }
}
