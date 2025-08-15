using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDrainages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StandId",
                table: "FormedDrainages");

            migrationBuilder.CreateTable(
                name: "StandDrainage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StandId = table.Column<int>(type: "integer", nullable: false),
                    DrainageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandDrainage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandDrainage_FormedDrainages_DrainageId",
                        column: x => x.DrainageId,
                        principalTable: "FormedDrainages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandDrainage_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StandDrainage_DrainageId",
                table: "StandDrainage",
                column: "DrainageId");

            migrationBuilder.CreateIndex(
                name: "IX_StandDrainage_StandId",
                table: "StandDrainage",
                column: "StandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandDrainage");

            migrationBuilder.AddColumn<int>(
                name: "StandId",
                table: "FormedDrainages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormedDrainages_StandId",
                table: "FormedDrainages",
                column: "StandId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormedDrainages_Stands_StandId",
                table: "FormedDrainages",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id");
        }
    }
}
