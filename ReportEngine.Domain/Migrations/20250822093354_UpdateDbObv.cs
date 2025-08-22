using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbObv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ObvyazkaType",
                table: "Stands",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<float>(
                name: "TreeSocketCount",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TreeSocketCount",
                table: "ObvyazkiInStands");

            migrationBuilder.AlterColumn<int>(
                name: "ObvyazkaType",
                table: "Stands",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
