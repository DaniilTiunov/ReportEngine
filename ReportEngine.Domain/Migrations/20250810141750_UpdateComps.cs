using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FrameComponent_FormedFrames_FormedFrameId",
                table: "FrameComponent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FrameComponent",
                table: "FrameComponent");

            migrationBuilder.RenameTable(
                name: "FrameComponent",
                newName: "FrameComponents");

            migrationBuilder.RenameIndex(
                name: "IX_FrameComponent_FormedFrameId",
                table: "FrameComponents",
                newName: "IX_FrameComponents_FormedFrameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FrameComponents",
                table: "FrameComponents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FrameComponents_FormedFrames_FormedFrameId",
                table: "FrameComponents",
                column: "FormedFrameId",
                principalTable: "FormedFrames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FrameComponents_FormedFrames_FormedFrameId",
                table: "FrameComponents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FrameComponents",
                table: "FrameComponents");

            migrationBuilder.RenameTable(
                name: "FrameComponents",
                newName: "FrameComponent");

            migrationBuilder.RenameIndex(
                name: "IX_FrameComponents_FormedFrameId",
                table: "FrameComponent",
                newName: "IX_FrameComponent_FormedFrameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FrameComponent",
                table: "FrameComponent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FrameComponent_FormedFrames_FormedFrameId",
                table: "FrameComponent",
                column: "FormedFrameId",
                principalTable: "FormedFrames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
