using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom_project_2024.Migrations
{
    /// <inheritdoc />
    public partial class addbedinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BedType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BedType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BedInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BedTypeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BedInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BedInfo_BedType_BedTypeId",
                        column: x => x.BedTypeId,
                        principalTable: "BedType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BedInfoHouse",
                columns: table => new
                {
                    BedsId = table.Column<int>(type: "int", nullable: false),
                    HousesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BedInfoHouse", x => new { x.BedsId, x.HousesId });
                    table.ForeignKey(
                        name: "FK_BedInfoHouse_BedInfo_BedsId",
                        column: x => x.BedsId,
                        principalTable: "BedInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BedInfoHouse_Houses_HousesId",
                        column: x => x.HousesId,
                        principalTable: "Houses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BedInfo_BedTypeId",
                table: "BedInfo",
                column: "BedTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BedInfoHouse_HousesId",
                table: "BedInfoHouse",
                column: "HousesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BedInfoHouse");

            migrationBuilder.DropTable(
                name: "BedInfo");

            migrationBuilder.DropTable(
                name: "BedType");
        }
    }
}
