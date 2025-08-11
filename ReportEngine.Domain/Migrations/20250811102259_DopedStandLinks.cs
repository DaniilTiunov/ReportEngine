using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class DopedStandLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormedDrainages_Stands_StandId",
                table: "FormedDrainages");

            migrationBuilder.AlterColumn<int>(
                name: "StandId",
                table: "FormedDrainages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_FormedDrainages_Stands_StandId",
                table: "FormedDrainages",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormedDrainages_Stands_StandId",
                table: "FormedDrainages");

            migrationBuilder.AlterColumn<int>(
                name: "StandId",
                table: "FormedDrainages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormedDrainages_Stands_StandId",
                table: "FormedDrainages",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
