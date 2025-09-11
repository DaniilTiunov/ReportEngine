using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateObvCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ArmatureCount",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "KMCHCount",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "MaterialLineCount",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TreeSocketMaterialCount",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmatureCount",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "KMCHCount",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "MaterialLineCount",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "TreeSocketMaterialCount",
                table: "ObvyazkiInStands");
        }
    }
}
