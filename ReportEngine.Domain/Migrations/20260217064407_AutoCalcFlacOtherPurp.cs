using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AutoCalcFlacOtherPurp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoCalculationEnabled",
                table: "DrainagePurposes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoCalculationEnabled",
                table: "AdditionalEquipPurposes",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoCalculationEnabled",
                table: "DrainagePurposes");

            migrationBuilder.DropColumn(
                name: "IsAutoCalculationEnabled",
                table: "AdditionalEquipPurposes");
        }
    }
}
