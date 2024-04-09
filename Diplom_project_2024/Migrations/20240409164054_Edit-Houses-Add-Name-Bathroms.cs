using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom_project_2024.Migrations
{
    /// <inheritdoc />
    public partial class EditHousesAddNameBathroms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rooms",
                table: "Houses");

            migrationBuilder.RenameColumn(
                name: "SquareMeter",
                table: "Houses",
                newName: "Bathrooms");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Houses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Houses");

            migrationBuilder.RenameColumn(
                name: "Bathrooms",
                table: "Houses",
                newName: "SquareMeter");

            migrationBuilder.AddColumn<int>(
                name: "Rooms",
                table: "Houses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
