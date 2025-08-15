using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateObvInStand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Armature",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstDescription",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NN",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SecondDescription",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdDescription",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Armature",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "FirstDescription",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "NN",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "SecondDescription",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "ThirdDescription",
                table: "ObvyazkiInStands");
        }
    }
}
