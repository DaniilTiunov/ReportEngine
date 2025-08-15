using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFrames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StandId",
                table: "FormedFrames");

            migrationBuilder.CreateTable(
                name: "StandFrames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StandId = table.Column<int>(type: "integer", nullable: false),
                    FrameId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandFrames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandFrames_FormedFrames_FrameId",
                        column: x => x.FrameId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandFrames_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StandFrames_FrameId",
                table: "StandFrames",
                column: "FrameId");

            migrationBuilder.CreateIndex(
                name: "IX_StandFrames_StandId",
                table: "StandFrames",
                column: "StandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandFrames");

            migrationBuilder.AddColumn<int>(
                name: "StandId",
                table: "FormedFrames",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormedFrames_StandId",
                table: "FormedFrames",
                column: "StandId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormedFrames_Stands_StandId",
                table: "FormedFrames",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id");
        }
    }
}
