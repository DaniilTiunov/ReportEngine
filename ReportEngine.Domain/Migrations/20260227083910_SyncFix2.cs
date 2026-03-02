using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class SyncFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "ContainerBatch",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),

                ProjectInfoId = table.Column<int>(nullable: false),
                BatchOrder = table.Column<int>(nullable: false),
                ContainersCount = table.Column<int>(nullable: false),
                StandsCount = table.Column<int>(nullable: false),
                Name = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ContainerBatch", x => x.Id);
            });

            migrationBuilder.CreateTable(
                name: "ContainerStand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),

                    ProjectInfoId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    StandsCount = table.Column<int>(nullable: false),
                    StandsWeight = table.Column<float>(nullable: false),
                    ContainerWeight = table.Column<float>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ContainerCost = table.Column<float>(nullable: true),
                    ContainerBatchId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerStand", x => x.Id);

                    table.ForeignKey(
                        name: "FK_ContainerStand_ContainerBatch_ContainerBatchId",
                        column: x => x.ContainerBatchId,
                        principalTable: "ContainerBatch",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerStand_ContainerBatchId",
                table: "ContainerStand",
                column: "ContainerBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
