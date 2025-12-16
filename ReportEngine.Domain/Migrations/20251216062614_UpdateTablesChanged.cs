using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EquipId",
                table: "TablesChanges",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NewName",
                table: "TablesChanges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OldName",
                table: "TablesChanges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "TablesChanges",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipId",
                table: "TablesChanges");

            migrationBuilder.DropColumn(
                name: "NewName",
                table: "TablesChanges");

            migrationBuilder.DropColumn(
                name: "OldName",
                table: "TablesChanges");

            migrationBuilder.DropColumn(
                name: "Processed",
                table: "TablesChanges");
        }
    }
}
