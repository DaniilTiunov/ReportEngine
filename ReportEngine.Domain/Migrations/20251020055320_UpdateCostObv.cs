using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCostObv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArmatureCostPerUnit",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KMCHCostPerUnit",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialLineCostPerUnit",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreeSocketMaterialCostPerUnit",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmatureCostPerUnit",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "KMCHCostPerUnit",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "MaterialLineCostPerUnit",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "TreeSocketMaterialCostPerUnit",
                table: "ObvyazkiInStands");
        }
    }
}
