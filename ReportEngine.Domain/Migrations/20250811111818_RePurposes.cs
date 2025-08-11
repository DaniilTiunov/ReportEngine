using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RePurposes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Material",
                table: "FormedDrainages");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "FormedDrainages");

            migrationBuilder.AddColumn<string>(
                name: "Material",
                table: "DrainagePurpose",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Quantity",
                table: "DrainagePurpose",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Material",
                table: "DrainagePurpose");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "DrainagePurpose");

            migrationBuilder.AddColumn<string>(
                name: "Material",
                table: "FormedDrainages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Quantity",
                table: "FormedDrainages",
                type: "real",
                nullable: true);
        }
    }
}
