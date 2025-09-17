using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ContainersBatchUpdatesJoska : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quary",
                table: "ContainersStand");

            migrationBuilder.RenameColumn(
                name: "Quary",
                table: "ContainersBatch",
                newName: "Name");

            migrationBuilder.AlterColumn<float>(
                name: "StandsWeight",
                table: "ContainersStand",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StandsCount",
                table: "ContainersStand",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StandsCount",
                table: "ContainersBatch",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContainersCount",
                table: "ContainersBatch",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BatchOrder",
                table: "ContainersBatch",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchOrder",
                table: "ContainersBatch");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ContainersBatch",
                newName: "Quary");

            migrationBuilder.AlterColumn<float>(
                name: "StandsWeight",
                table: "ContainersStand",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "StandsCount",
                table: "ContainersStand",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Quary",
                table: "ContainersStand",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StandsCount",
                table: "ContainersBatch",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ContainersCount",
                table: "ContainersBatch",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
