using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ContainersBatchUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Quary",
                table: "ContainersBatch",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quary",
                table: "ContainersBatch");
        }
    }
}
