using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormedFrameId",
                table: "PillarEqiups");

            migrationBuilder.DropColumn(
                name: "FormedFrameId",
                table: "FrameRolls");

            migrationBuilder.DropColumn(
                name: "FormedFrameId",
                table: "FrameDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormedFrameId",
                table: "PillarEqiups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FormedFrameId",
                table: "FrameRolls",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FormedFrameId",
                table: "FrameDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
