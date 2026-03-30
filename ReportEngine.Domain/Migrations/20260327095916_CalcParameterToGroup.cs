using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CalcParameterToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_Calculatio~",
                table: "CalculationParameters");

            migrationBuilder.AlterColumn<int>(
                name: "CalculationParameterGroupId",
                table: "CalculationParameters",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParameterGroupId",
                table: "CalculationParameters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_Calculatio~",
                table: "CalculationParameters",
                column: "CalculationParameterGroupId",
                principalTable: "CalculationParametersGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_Calculatio~",
                table: "CalculationParameters");

            migrationBuilder.DropColumn(
                name: "ParameterGroupId",
                table: "CalculationParameters");

            migrationBuilder.AlterColumn<int>(
                name: "CalculationParameterGroupId",
                table: "CalculationParameters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_CalculationParameters_CalculationParametersGroup_Calculatio~",
                table: "CalculationParameters",
                column: "CalculationParameterGroupId",
                principalTable: "CalculationParametersGroup",
                principalColumn: "Id");
        }
    }
}
