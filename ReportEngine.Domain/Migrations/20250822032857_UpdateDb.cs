using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TreeSocket",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "NN",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialLine",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "KMCH",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Armature",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<float>(
                name: "Clamp",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "HumanCost",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "LineLength",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "ObvyazkaName",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OtherLineCount",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sensor",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SensorType",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "WidthOnFrame",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ZraCount",
                table: "ObvyazkiInStands",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clamp",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "HumanCost",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "LineLength",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "ObvyazkaName",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "OtherLineCount",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "Sensor",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "SensorType",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "WidthOnFrame",
                table: "ObvyazkiInStands");

            migrationBuilder.DropColumn(
                name: "ZraCount",
                table: "ObvyazkiInStands");

            migrationBuilder.AlterColumn<string>(
                name: "TreeSocket",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NN",
                table: "ObvyazkiInStands",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaterialLine",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KMCH",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Armature",
                table: "ObvyazkiInStands",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
