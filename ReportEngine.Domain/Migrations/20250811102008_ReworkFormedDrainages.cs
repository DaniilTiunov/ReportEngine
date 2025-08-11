using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ReworkFormedDrainages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StandId",
                table: "Obvyazki",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandId",
                table: "FormedFrames",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormedDrainages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StandId = table.Column<int>(type: "integer", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    Material = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormedDrainages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormedDrainages_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Obvyazki_StandId",
                table: "Obvyazki",
                column: "StandId");

            migrationBuilder.CreateIndex(
                name: "IX_FormedFrames_StandId",
                table: "FormedFrames",
                column: "StandId");

            migrationBuilder.CreateIndex(
                name: "IX_FormedDrainages_StandId",
                table: "FormedDrainages",
                column: "StandId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormedFrames_Stands_StandId",
                table: "FormedFrames",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Obvyazki_Stands_StandId",
                table: "Obvyazki",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormedFrames_Stands_StandId",
                table: "FormedFrames");

            migrationBuilder.DropForeignKey(
                name: "FK_Obvyazki_Stands_StandId",
                table: "Obvyazki");

            migrationBuilder.DropTable(
                name: "FormedDrainages");

            migrationBuilder.DropIndex(
                name: "IX_Obvyazki_StandId",
                table: "Obvyazki");

            migrationBuilder.DropIndex(
                name: "IX_FormedFrames_StandId",
                table: "FormedFrames");

            migrationBuilder.DropColumn(
                name: "StandId",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "StandId",
                table: "FormedFrames");
        }
    }
}
