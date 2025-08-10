using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ReworkFrames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormedFrameFrameDetail");

            migrationBuilder.DropTable(
                name: "FormedFrameFrameRoll");

            migrationBuilder.DropTable(
                name: "FormedFramePillarEqiup");

            migrationBuilder.CreateTable(
                name: "FrameComponent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormedFrameId = table.Column<int>(type: "integer", nullable: false),
                    ComponentId = table.Column<int>(type: "integer", nullable: false),
                    ComponentType = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FrameComponent_FormedFrames_FormedFrameId",
                        column: x => x.FormedFrameId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FrameComponent_FormedFrameId",
                table: "FrameComponent",
                column: "FormedFrameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FrameComponent");

            migrationBuilder.CreateTable(
                name: "FormedFrameFrameDetail",
                columns: table => new
                {
                    FormedFramesId = table.Column<int>(type: "integer", nullable: false),
                    FrameDetailsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormedFrameFrameDetail", x => new { x.FormedFramesId, x.FrameDetailsId });
                    table.ForeignKey(
                        name: "FK_FormedFrameFrameDetail_FormedFrames_FormedFramesId",
                        column: x => x.FormedFramesId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormedFrameFrameDetail_FrameDetails_FrameDetailsId",
                        column: x => x.FrameDetailsId,
                        principalTable: "FrameDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormedFrameFrameRoll",
                columns: table => new
                {
                    FormedFramesId = table.Column<int>(type: "integer", nullable: false),
                    FrameRollsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormedFrameFrameRoll", x => new { x.FormedFramesId, x.FrameRollsId });
                    table.ForeignKey(
                        name: "FK_FormedFrameFrameRoll_FormedFrames_FormedFramesId",
                        column: x => x.FormedFramesId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormedFrameFrameRoll_FrameRolls_FrameRollsId",
                        column: x => x.FrameRollsId,
                        principalTable: "FrameRolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormedFramePillarEqiup",
                columns: table => new
                {
                    FormedFramesId = table.Column<int>(type: "integer", nullable: false),
                    PillarEqiupsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormedFramePillarEqiup", x => new { x.FormedFramesId, x.PillarEqiupsId });
                    table.ForeignKey(
                        name: "FK_FormedFramePillarEqiup_FormedFrames_FormedFramesId",
                        column: x => x.FormedFramesId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormedFramePillarEqiup_PillarEqiups_PillarEqiupsId",
                        column: x => x.PillarEqiupsId,
                        principalTable: "PillarEqiups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormedFrameFrameDetail_FrameDetailsId",
                table: "FormedFrameFrameDetail",
                column: "FrameDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_FormedFrameFrameRoll_FrameRollsId",
                table: "FormedFrameFrameRoll",
                column: "FrameRollsId");

            migrationBuilder.CreateIndex(
                name: "IX_FormedFramePillarEqiup_PillarEqiupsId",
                table: "FormedFramePillarEqiup",
                column: "PillarEqiupsId");
        }
    }
}
