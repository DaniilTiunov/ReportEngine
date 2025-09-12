using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateObvyazki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArmatureMeasure",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KMCHMeasure",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialLineMeasure",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreeSocketMaterialMeasure",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmatureMeasure",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "KMCHMeasure",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "MaterialLineMeasure",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "TreeSocketMaterialMeasure",
                table: "ObvyazkiInStands");
        }
    }
}
