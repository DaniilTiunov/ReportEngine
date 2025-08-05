using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStandsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DesigneStand",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KKSCounter",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarkMinus",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarkPlus",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sensor",
                table: "Stands",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesigneStand",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "KKSCounter",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "MarkMinus",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "MarkPlus",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Sensor",
                table: "Stands");
        }
    }
}
