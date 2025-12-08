using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedObvEquip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObvyazkaAdditionalEquipPurpose",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    Material = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<float>(type: "real", nullable: true),
                    CostPerUnit = table.Column<float>(type: "real", nullable: true),
                    Measure = table.Column<string>(type: "text", nullable: true),
                    ExportDays = table.Column<int>(type: "integer", nullable: true),
                    ObvyazkaInStandId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObvyazkaAdditionalEquipPurpose", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObvyazkaAdditionalEquipPurpose_ObvyazkiInStands_ObvyazkaInS~",
                        column: x => x.ObvyazkaInStandId,
                        principalTable: "ObvyazkiInStands",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObvyazkaAdditionalEquipPurpose_ObvyazkaInStandId",
                table: "ObvyazkaAdditionalEquipPurpose",
                column: "ObvyazkaInStandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObvyazkaAdditionalEquipPurpose");
        }
    }
}
