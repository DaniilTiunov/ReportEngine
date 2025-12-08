using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExportDaysEquips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArmatureExportDays",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KMCHExportDays",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaterialLineExportDays",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TreeSocketExportDays",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExportDays",
                table: "ElectricalPurposes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExportDays",
                table: "DrainagePurposes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExportDays",
                table: "AdditionalEquipPurposes",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmatureExportDays",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "KMCHExportDays",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "MaterialLineExportDays",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "TreeSocketExportDays",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "ExportDays",
                table: "ElectricalPurposes");

            migrationBuilder.DropColumn(
                name: "ExportDays",
                table: "DrainagePurposes");

            migrationBuilder.DropColumn(
                name: "ExportDays",
                table: "AdditionalEquipPurposes");
        }
    }
}
