using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "SensorsBraces",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "PillarEqiups",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "ObvyazkaAdditionalEquipPurpose",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "Heaters",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "FrameRolls",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "FrameDetails",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "ElectricalPurposes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "DrainagePurposes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "DrainageBraces",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "CabelProtections",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "CabelProductions",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "CabelInputs",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "CabelBoxes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "BoxesBraces",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "AdditionalEquipPurposes",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "SensorsBraces");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "PillarEqiups");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "ObvyazkaAdditionalEquipPurpose");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Heaters");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "FrameRolls");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "FrameDetails");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "ElectricalPurposes");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "DrainagePurposes");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "DrainageBraces");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "CabelProtections");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "CabelProductions");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "CabelInputs");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "CabelBoxes");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "BoxesBraces");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "AdditionalEquipPurposes");
        }
    }
}
