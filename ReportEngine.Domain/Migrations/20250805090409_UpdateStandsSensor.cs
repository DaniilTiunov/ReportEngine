using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStandsSensor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sensor",
                table: "Stands",
                newName: "FirstSensorType");

            migrationBuilder.RenameColumn(
                name: "MarkPlus",
                table: "Stands",
                newName: "FirstSensorMarkPlus");

            migrationBuilder.RenameColumn(
                name: "MarkMinus",
                table: "Stands",
                newName: "FirstSensorMarkMinus");

            migrationBuilder.RenameColumn(
                name: "KKSCounter",
                table: "Stands",
                newName: "FirstSensorKKS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstSensorType",
                table: "Stands",
                newName: "Sensor");

            migrationBuilder.RenameColumn(
                name: "FirstSensorMarkPlus",
                table: "Stands",
                newName: "MarkPlus");

            migrationBuilder.RenameColumn(
                name: "FirstSensorMarkMinus",
                table: "Stands",
                newName: "MarkMinus");

            migrationBuilder.RenameColumn(
                name: "FirstSensorKKS",
                table: "Stands",
                newName: "KKSCounter");
        }
    }
}
