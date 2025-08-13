using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateObv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstSensorKKS",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "FirstSensorMarkMinus",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "FirstSensorMarkPlus",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "FirstSensorType",
                table: "Stands");

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

            migrationBuilder.AddColumn<string>(
                name: "FirstSensorKKS",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstSensorMarkMinus",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstSensorMarkPlus",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstSensorType",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondSensorKKS",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondSensorMarkMinus",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondSensorMarkPlus",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondSensorType",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdSensorKKS",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdSensorMarkMinus",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdSensorMarkPlus",
                table: "Obvyazki",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdSensorType",
                table: "Obvyazki",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstSensorKKS",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "FirstSensorMarkMinus",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "FirstSensorMarkPlus",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "FirstSensorType",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "SecondSensorKKS",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "SecondSensorMarkMinus",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "SecondSensorMarkPlus",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "SecondSensorType",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "ThirdSensorKKS",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "ThirdSensorMarkMinus",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "ThirdSensorMarkPlus",
                table: "Obvyazki");

            migrationBuilder.DropColumn(
                name: "ThirdSensorType",
                table: "Obvyazki");

            migrationBuilder.AddColumn<string>(
                name: "FirstSensorKKS",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstSensorMarkMinus",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstSensorMarkPlus",
                table: "Stands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstSensorType",
                table: "Stands",
                type: "text",
                nullable: true);

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
        }
    }
}
