using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    public partial class RemoveStandIdFromFormedDrainages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormedDrainages_Stands_StandId",
                table: "FormedDrainages");

            migrationBuilder.DropColumn(
                name: "StandId",
                table: "FormedDrainages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StandId",
                table: "FormedDrainages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormedDrainages_Stands_StandId",
                table: "FormedDrainages",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}