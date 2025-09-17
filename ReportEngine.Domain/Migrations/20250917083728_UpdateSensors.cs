using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSensors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThirdDescription",
                table: "ObvyazkiInStands",
                newName: "ThirdSensorDescription");

            migrationBuilder.RenameColumn(
                name: "SecondDescription",
                table: "ObvyazkiInStands",
                newName: "SecondSensorDescription");

            migrationBuilder.RenameColumn(
                name: "FirstDescription",
                table: "ObvyazkiInStands",
                newName: "FirstSensorDescription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThirdSensorDescription",
                table: "ObvyazkiInStands",
                newName: "ThirdDescription");

            migrationBuilder.RenameColumn(
                name: "SecondSensorDescription",
                table: "ObvyazkiInStands",
                newName: "SecondDescription");

            migrationBuilder.RenameColumn(
                name: "FirstSensorDescription",
                table: "ObvyazkiInStands",
                newName: "FirstDescription");
        }
    }
}
