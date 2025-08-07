using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFormedFrameTable3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormedFrames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FrameType = table.Column<string>(type: "text", nullable: false),
                    Width = table.Column<float>(type: "real", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    Depth = table.Column<float>(type: "real", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Designe = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormedFrames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseElectricComponent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<float>(type: "real", nullable: false),
                    Measure = table.Column<string>(type: "text", nullable: false),
                    Cabel = table.Column<int>(type: "integer", nullable: false),
                    ElectricProtection = table.Column<int>(type: "integer", nullable: false),
                    CabelInput = table.Column<int>(type: "integer", nullable: false),
                    ExportDays = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    FormedFrameId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseElectricComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseElectricComponent_FormedFrames_FormedFrameId",
                        column: x => x.FormedFrameId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BaseEquip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<float>(type: "real", nullable: false),
                    Measure = table.Column<string>(type: "text", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    Width = table.Column<float>(type: "real", nullable: false),
                    Depth = table.Column<float>(type: "real", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    ExportDays = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    FormedFrameId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEquip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseEquip_FormedFrames_FormedFrameId",
                        column: x => x.FormedFrameId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BaseFrame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<float>(type: "real", nullable: false),
                    Measure = table.Column<string>(type: "text", nullable: false),
                    ExportDays = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    FormedFrameId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseFrame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseFrame_FormedFrames_FormedFrameId",
                        column: x => x.FormedFrameId,
                        principalTable: "FormedFrames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseElectricComponent_FormedFrameId",
                table: "BaseElectricComponent",
                column: "FormedFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseEquip_FormedFrameId",
                table: "BaseEquip",
                column: "FormedFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseFrame_FormedFrameId",
                table: "BaseFrame",
                column: "FormedFrameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseElectricComponent");

            migrationBuilder.DropTable(
                name: "BaseEquip");

            migrationBuilder.DropTable(
                name: "BaseFrame");

            migrationBuilder.DropTable(
                name: "FormedFrames");
        }
    }
}