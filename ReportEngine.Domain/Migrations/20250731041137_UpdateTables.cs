using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isGalvanized",
                table: "Projects",
                newName: "IsGalvanized");

            migrationBuilder.AlterColumn<string>(
                name: "KKSCode",
                table: "Stands",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Design",
                table: "Stands",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Armature",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BraceType",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Devices",
                table: "Stands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "KMCH",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialLine",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NN",
                table: "Stands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ObvyazkaType",
                table: "Stands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StandSummCost",
                table: "Stands",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TreeScoket",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "Stands",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "Stands",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Armature",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "BraceType",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Devices",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "KMCH",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "MaterialLine",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "NN",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ObvyazkaType",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "StandSummCost",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "TreeScoket",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Stands");

            migrationBuilder.RenameColumn(
                name: "IsGalvanized",
                table: "Projects",
                newName: "isGalvanized");

            migrationBuilder.AlterColumn<string>(
                name: "KKSCode",
                table: "Stands",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Design",
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
