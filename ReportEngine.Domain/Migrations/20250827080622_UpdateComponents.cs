using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ObvyazkaType",
                table: "Stands",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<float>(
                name: "CostPerUnit",
                table: "ElectricalPurposes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CostPerUnit",
                table: "DrainagePurposes",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CostPerUnit",
                table: "AdditionalEquipPurposes",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPerUnit",
                table: "ElectricalPurposes");

            migrationBuilder.DropColumn(
                name: "CostPerUnit",
                table: "DrainagePurposes");

            migrationBuilder.DropColumn(
                name: "CostPerUnit",
                table: "AdditionalEquipPurposes");

            migrationBuilder.AlterColumn<string>(
                name: "ObvyazkaType",
                table: "Stands",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
