using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CollectionsGroupAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CalculationParameterGroupId",
                table: "CalculationParameters",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CalculationParametersGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SettingsType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationParametersGroup", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculationParameters_CalculationParameterGroupId",
                table: "CalculationParameters",
                column: "CalculationParameterGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_Calculatio~",
                table: "CalculationParameters",
                column: "CalculationParameterGroupId",
                principalTable: "CalculationParametersGroup",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_Calculatio~",
                table: "CalculationParameters");

            migrationBuilder.DropTable(
                name: "CalculationParametersGroup");

            migrationBuilder.DropIndex(
                name: "IX_CalculationParameters_CalculationParameterGroupId",
                table: "CalculationParameters");

            migrationBuilder.DropColumn(
                name: "CalculationParameterGroupId",
                table: "CalculationParameters");
        }
    }
}
