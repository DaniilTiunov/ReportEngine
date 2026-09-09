using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ObvInStandsOuterEntityAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArmatureId",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArmatureType",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KMCHId",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KMCHType",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaterialLineId",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialLineType",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TreeSocketId",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreeSocketType",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmatureId",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "ArmatureType",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "KMCHId",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "KMCHType",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "MaterialLineId",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "MaterialLineType",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "TreeSocketId",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "TreeSocketType",
                table: "ObvyazkiInStands");
        }
    }
}
