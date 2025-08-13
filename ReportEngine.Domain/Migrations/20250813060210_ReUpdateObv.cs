using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ReUpdateObv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obvyazki_Stands_StandId",
                table: "Obvyazki");

            migrationBuilder.DropIndex(
                name: "IX_Obvyazki_StandId",
                table: "Obvyazki");

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
                name: "StandId",
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

            migrationBuilder.CreateTable(
                name: "ObvyazkiInStands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StandId = table.Column<int>(type: "integer", nullable: false),
                    ObvyazkaId = table.Column<int>(type: "integer", nullable: false),
                    MaterialLine = table.Column<string>(type: "text", nullable: false),
                    TreeSocket = table.Column<string>(type: "text", nullable: false),
                    KMCH = table.Column<string>(type: "text", nullable: false),
                    FirstSensorType = table.Column<string>(type: "text", nullable: true),
                    FirstSensorKKS = table.Column<string>(type: "text", nullable: true),
                    FirstSensorMarkPlus = table.Column<string>(type: "text", nullable: true),
                    FirstSensorMarkMinus = table.Column<string>(type: "text", nullable: true),
                    SecondSensorType = table.Column<string>(type: "text", nullable: true),
                    SecondSensorKKS = table.Column<string>(type: "text", nullable: true),
                    SecondSensorMarkPlus = table.Column<string>(type: "text", nullable: true),
                    SecondSensorMarkMinus = table.Column<string>(type: "text", nullable: true),
                    ThirdSensorType = table.Column<string>(type: "text", nullable: true),
                    ThirdSensorKKS = table.Column<string>(type: "text", nullable: true),
                    ThirdSensorMarkPlus = table.Column<string>(type: "text", nullable: true),
                    ThirdSensorMarkMinus = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObvyazkiInStands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObvyazkiInStands_Obvyazki_ObvyazkaId",
                        column: x => x.ObvyazkaId,
                        principalTable: "Obvyazki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObvyazkiInStands_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObvyazkiInStands_ObvyazkaId",
                table: "ObvyazkiInStands",
                column: "ObvyazkaId");

            migrationBuilder.CreateIndex(
                name: "IX_ObvyazkiInStands_StandId",
                table: "ObvyazkiInStands",
                column: "StandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObvyazkiInStands");

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

            migrationBuilder.AddColumn<int>(
                name: "StandId",
                table: "Obvyazki",
                type: "integer",
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

            migrationBuilder.CreateIndex(
                name: "IX_Obvyazki_StandId",
                table: "Obvyazki",
                column: "StandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Obvyazki_Stands_StandId",
                table: "Obvyazki",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id");
        }
    }
}
