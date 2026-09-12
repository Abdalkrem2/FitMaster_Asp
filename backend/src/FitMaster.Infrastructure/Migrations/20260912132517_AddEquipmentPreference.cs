using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "equipment_preference",
                table: "member_profiles",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Gym");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "equipment_preference",
                table: "member_profiles");
        }
    }
}
