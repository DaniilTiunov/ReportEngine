using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateObvAddEquips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObvyazkaAdditionalEquipPurpose_ObvyazkiInStands_ObvyazkaInS~",
                table: "ObvyazkaAdditionalEquipPurpose");

            migrationBuilder.AddForeignKey(
                name: "FK_ObvyazkaAdditionalEquipPurpose_ObvyazkiInStands_ObvyazkaInS~",
                table: "ObvyazkaAdditionalEquipPurpose",
                column: "ObvyazkaInStandId",
                principalTable: "ObvyazkiInStands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObvyazkaAdditionalEquipPurpose_ObvyazkiInStands_ObvyazkaInS~",
                table: "ObvyazkaAdditionalEquipPurpose");

            migrationBuilder.AddForeignKey(
                name: "FK_ObvyazkaAdditionalEquipPurpose_ObvyazkiInStands_ObvyazkaInS~",
                table: "ObvyazkaAdditionalEquipPurpose",
                column: "ObvyazkaInStandId",
                principalTable: "ObvyazkiInStands",
                principalColumn: "Id");
        }
    }
}
