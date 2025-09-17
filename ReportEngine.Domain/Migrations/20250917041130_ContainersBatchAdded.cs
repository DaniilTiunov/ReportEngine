using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ContainersBatchAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContainerStand_Projects_ProjectInfoId",
                table: "ContainerStand");

            migrationBuilder.DropForeignKey(
                name: "FK_Stands_ContainerStand_ContainerStandId",
                table: "Stands");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContainerStand",
                table: "ContainerStand");

            migrationBuilder.RenameTable(
                name: "ContainerStand",
                newName: "ContainersStand");

            migrationBuilder.RenameIndex(
                name: "IX_ContainerStand_ProjectInfoId",
                table: "ContainersStand",
                newName: "IX_ContainersStand_ProjectInfoId");

            migrationBuilder.AddColumn<int>(
                name: "ContainerBatchId",
                table: "ContainersStand",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quary",
                table: "ContainersStand",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContainersStand",
                table: "ContainersStand",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ContainersBatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectInfoId = table.Column<int>(type: "integer", nullable: false),
                    ContainersCount = table.Column<int>(type: "integer", nullable: true),
                    StandsCount = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainersBatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainersBatch_Projects_ProjectInfoId",
                        column: x => x.ProjectInfoId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContainersStand_ContainerBatchId",
                table: "ContainersStand",
                column: "ContainerBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainersBatch_ProjectInfoId",
                table: "ContainersBatch",
                column: "ProjectInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContainersStand_ContainersBatch_ContainerBatchId",
                table: "ContainersStand",
                column: "ContainerBatchId",
                principalTable: "ContainersBatch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContainersStand_Projects_ProjectInfoId",
                table: "ContainersStand",
                column: "ProjectInfoId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stands_ContainersStand_ContainerStandId",
                table: "Stands",
                column: "ContainerStandId",
                principalTable: "ContainersStand",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContainersStand_ContainersBatch_ContainerBatchId",
                table: "ContainersStand");

            migrationBuilder.DropForeignKey(
                name: "FK_ContainersStand_Projects_ProjectInfoId",
                table: "ContainersStand");

            migrationBuilder.DropForeignKey(
                name: "FK_Stands_ContainersStand_ContainerStandId",
                table: "Stands");

            migrationBuilder.DropTable(
                name: "ContainersBatch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContainersStand",
                table: "ContainersStand");

            migrationBuilder.DropIndex(
                name: "IX_ContainersStand_ContainerBatchId",
                table: "ContainersStand");

            migrationBuilder.DropColumn(
                name: "ContainerBatchId",
                table: "ContainersStand");

            migrationBuilder.DropColumn(
                name: "Quary",
                table: "ContainersStand");

            migrationBuilder.RenameTable(
                name: "ContainersStand",
                newName: "ContainerStand");

            migrationBuilder.RenameIndex(
                name: "IX_ContainersStand_ProjectInfoId",
                table: "ContainerStand",
                newName: "IX_ContainerStand_ProjectInfoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContainerStand",
                table: "ContainerStand",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContainerStand_Projects_ProjectInfoId",
                table: "ContainerStand",
                column: "ProjectInfoId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stands_ContainerStand_ContainerStandId",
                table: "Stands",
                column: "ContainerStandId",
                principalTable: "ContainerStand",
                principalColumn: "Id");
        }
    }
}
