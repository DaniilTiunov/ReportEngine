using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class foreingKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_Calculatio~",
                table: "CalculationParameters");

            migrationBuilder.DropIndex(
                name: "IX_CalculationParameters_CalculationParameterGroupId",
                table: "CalculationParameters");

            migrationBuilder.DropColumn(
                name: "CalculationParameterGroupId",
                table: "CalculationParameters");

            migrationBuilder.CreateIndex(
                name: "IX_CalculationParameters_ParameterGroupId",
                table: "CalculationParameters",
                column: "ParameterGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_ParameterG~",
                table: "CalculationParameters",
                column: "ParameterGroupId",
                principalTable: "CalculationParametersGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_ParameterG~",
                table: "CalculationParameters");

            migrationBuilder.DropIndex(
                name: "IX_CalculationParameters_ParameterGroupId",
                table: "CalculationParameters");

            migrationBuilder.AddColumn<int>(
                name: "CalculationParameterGroupId",
                table: "CalculationParameters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CalculationParameters_CalculationParameterGroupId",
                table: "CalculationParameters",
                column: "CalculationParameterGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_Calculatio~",
                table: "CalculationParameters",
                column: "CalculationParameterGroupId",
                principalTable: "CalculationParametersGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
