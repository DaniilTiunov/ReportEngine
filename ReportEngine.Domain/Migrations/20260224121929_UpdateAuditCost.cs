using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "NewCost",
                table: "TablesChanges",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "OldCost",
                table: "TablesChanges",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewCost",
                table: "TablesChanges");

            migrationBuilder.DropColumn(
                name: "OldCost",
                table: "TablesChanges");
        }
    }
}
