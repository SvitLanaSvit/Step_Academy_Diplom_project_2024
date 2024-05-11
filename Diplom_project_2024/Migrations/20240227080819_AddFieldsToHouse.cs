using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom_project_2024.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToHouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChildrenSleepingPlaces",
                table: "Houses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SleepingPlaces",
                table: "Houses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChildrenSleepingPlaces",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "SleepingPlaces",
                table: "Houses");
        }
    }
}
