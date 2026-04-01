using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CalcsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Depth",
                table: "StainlessSockets");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "StainlessSockets");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StainlessSockets");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "StainlessSockets");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "StainlessPipes");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "StainlessPipes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StainlessPipes");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "StainlessPipes");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "StainlessArmatures");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "StainlessArmatures");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StainlessArmatures");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "StainlessArmatures");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "Others");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Others");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Others");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "HeaterSockets");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "HeaterSockets");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "HeaterSockets");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "HeaterSockets");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "HeaterPipes");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "HeaterPipes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "HeaterPipes");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "HeaterPipes");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "HeaterArmatures");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "HeaterArmatures");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "HeaterArmatures");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "HeaterArmatures");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "Drainages");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Drainages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Drainages");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Drainages");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "CarbonSockets");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "CarbonSockets");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CarbonSockets");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "CarbonSockets");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "CarbonPipes");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "CarbonPipes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CarbonPipes");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "CarbonPipes");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "CarbonArmatures");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "CarbonArmatures");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CarbonArmatures");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "CarbonArmatures");

            migrationBuilder.AddColumn<int>(
                name: "EquipReferenceId",
                table: "CalculationParameters",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipReferenceType",
                table: "CalculationParameters",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipReferenceId",
                table: "CalculationParameters");

            migrationBuilder.DropColumn(
                name: "EquipReferenceType",
                table: "CalculationParameters");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "CalculationParameters");

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "StainlessSockets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "StainlessSockets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "StainlessSockets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "StainlessSockets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "StainlessPipes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "StainlessPipes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "StainlessPipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "StainlessPipes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "StainlessArmatures",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "StainlessArmatures",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "StainlessArmatures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "StainlessArmatures",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "Others",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "Others",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "Others",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "HeaterSockets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "HeaterSockets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "HeaterSockets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "HeaterSockets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "HeaterPipes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "HeaterPipes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "HeaterPipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "HeaterPipes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "HeaterArmatures",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "HeaterArmatures",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "HeaterArmatures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "HeaterArmatures",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "Drainages",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "Drainages",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Drainages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "Drainages",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "Containers",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "Containers",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "Containers",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "CarbonSockets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "CarbonSockets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CarbonSockets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "CarbonSockets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "CarbonPipes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "CarbonPipes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CarbonPipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "CarbonPipes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Depth",
                table: "CarbonArmatures",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "CarbonArmatures",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CarbonArmatures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "CarbonArmatures",
                type: "real",
                nullable: true);
        }
    }
}
