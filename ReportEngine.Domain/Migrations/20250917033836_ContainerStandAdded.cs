using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ContainerStandAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContainerStandId",
                table: "Stands",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContainerStand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectInfoId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    StandsWeight = table.Column<float>(type: "real", nullable: true),
                    StandsCount = table.Column<int>(type: "integer", nullable: true),
                    ContainerWeight = table.Column<float>(type: "real", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerStand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerStand_Projects_ProjectInfoId",
                        column: x => x.ProjectInfoId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stands_ContainerStandId",
                table: "Stands",
                column: "ContainerStandId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerStand_ProjectInfoId",
                table: "ContainerStand",
                column: "ProjectInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stands_ContainerStand_ContainerStandId",
                table: "Stands",
                column: "ContainerStandId",
                principalTable: "ContainerStand",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stands_ContainerStand_ContainerStandId",
                table: "Stands");

            migrationBuilder.DropTable(
                name: "ContainerStand");

            migrationBuilder.DropIndex(
                name: "IX_Stands_ContainerStandId",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ContainerStandId",
                table: "Stands");
        }
    }
}
