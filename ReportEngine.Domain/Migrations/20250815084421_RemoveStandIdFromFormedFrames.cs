using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    public partial class RemoveStandIdFromFormedFrames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormedFrames_Stands_StandId",
                table: "FormedFrames");

            migrationBuilder.DropColumn(
                name: "StandId",
                table: "FormedFrames");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StandId",
                table: "FormedFrames",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormedFrames_Stands_StandId",
                table: "FormedFrames",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}