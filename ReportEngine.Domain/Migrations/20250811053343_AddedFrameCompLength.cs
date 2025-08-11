using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedFrameCompLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecondSensorKKS",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondSensorMarkMinus",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondSensorMarkPlus",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondSensorType",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdSensorKKS",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdSensorMarkMinus",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdSensorMarkPlus",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdSensorType",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Length",
                table: "FrameComponents",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondSensorKKS",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "SecondSensorMarkMinus",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "SecondSensorMarkPlus",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "SecondSensorType",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ThirdSensorKKS",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ThirdSensorMarkMinus",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ThirdSensorMarkPlus",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ThirdSensorType",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "FrameComponents");
        }
    }
}
