using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFormedFrameTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FrameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormedFrameId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<float>(type: "real", nullable: false),
                    Measure = table.Column<string>(type: "text", nullable: false),
                    ExportDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FrameDetails_FormedFrames_FormedFrameId",
                        column: x => x.FormedFrameId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FrameRolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormedFrameId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<float>(type: "real", nullable: false),
                    Measure = table.Column<string>(type: "text", nullable: false),
                    ExportDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameRolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FrameRolls_FormedFrames_FormedFrameId",
                        column: x => x.FormedFrameId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PillarEqiups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormedFrameId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<float>(type: "real", nullable: false),
                    Measure = table.Column<string>(type: "text", nullable: false),
                    ExportDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PillarEqiups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PillarEqiups_FormedFrames_FormedFrameId",
                        column: x => x.FormedFrameId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FrameDetails_FormedFrameId",
                table: "FrameDetails",
                column: "FormedFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_FrameRolls_FormedFrameId",
                table: "FrameRolls",
                column: "FormedFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_PillarEqiups_FormedFrameId",
                table: "PillarEqiups",
                column: "FormedFrameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FrameDetails");

            migrationBuilder.DropTable(
                name: "FrameRolls");

            migrationBuilder.DropTable(
                name: "PillarEqiups");
        }
    }
}