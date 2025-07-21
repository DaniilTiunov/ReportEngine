using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddObvyazki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {                      
            migrationBuilder.CreateTable(
                name: "Obvyazki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    LineLength = table.Column<float>(type: "real", nullable: false),
                    ZraCount = table.Column<float>(type: "real", nullable: false),
                    TreeSocket = table.Column<float>(type: "real", nullable: false),
                    Sensor = table.Column<int>(type: "integer", nullable: false),
                    SensorType = table.Column<string>(type: "text", nullable: false),
                    Clamp = table.Column<float>(type: "real", nullable: false),
                    WidthOnFrame = table.Column<float>(type: "real", nullable: false),
                    OtherLineCount = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    HumanCost = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obvyazki", x => x.Id);
                });
            
            migrationBuilder.CreateTable(
                name: "Stands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stands", x => x.Id);
                });
                        
            migrationBuilder.CreateIndex(
                name: "IX_Projects_StandId",
                table: "Projects",
                column: "StandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
