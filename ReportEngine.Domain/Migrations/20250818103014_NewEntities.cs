using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    /// <inheritdoc />
    public partial class NewEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DrainagePurpose_FormedDrainages_FormedDrainageId",
                table: "DrainagePurpose");

            migrationBuilder.DropForeignKey(
                name: "FK_StandDrainage_FormedDrainages_DrainageId",
                table: "StandDrainage");

            migrationBuilder.DropForeignKey(
                name: "FK_StandDrainage_Stands_StandId",
                table: "StandDrainage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StandDrainage",
                table: "StandDrainage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DrainagePurpose",
                table: "DrainagePurpose");

            migrationBuilder.RenameTable(
                name: "StandDrainage",
                newName: "StandDrainages");

            migrationBuilder.RenameTable(
                name: "DrainagePurpose",
                newName: "DrainagePurposes");

            migrationBuilder.RenameIndex(
                name: "IX_StandDrainage_StandId",
                table: "StandDrainages",
                newName: "IX_StandDrainages_StandId");

            migrationBuilder.RenameIndex(
                name: "IX_StandDrainage_DrainageId",
                table: "StandDrainages",
                newName: "IX_StandDrainages_DrainageId");

            migrationBuilder.RenameIndex(
                name: "IX_DrainagePurpose_FormedDrainageId",
                table: "DrainagePurposes",
                newName: "IX_DrainagePurposes_FormedDrainageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StandDrainages",
                table: "StandDrainages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DrainagePurposes",
                table: "DrainagePurposes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FormedAdditionalEquips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormedAdditionalEquips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormedElectricalComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormedElectricalComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalEquipPurposes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    Material = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<float>(type: "real", nullable: true),
                    FormedAdditionalEquipId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalEquipPurposes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionalEquipPurposes_FormedAdditionalEquips_FormedAdditi~",
                        column: x => x.FormedAdditionalEquipId,
                        principalTable: "FormedAdditionalEquips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandAdditionalEquips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StandId = table.Column<int>(type: "integer", nullable: false),
                    AdditionalEquipId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandAdditionalEquips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandAdditionalEquips_FormedAdditionalEquips_AdditionalEqui~",
                        column: x => x.AdditionalEquipId,
                        principalTable: "FormedAdditionalEquips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandAdditionalEquips_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectricalPurposes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    Material = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<float>(type: "real", nullable: true),
                    FormedElectricalComponentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectricalPurposes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectricalPurposes_FormedElectricalComponents_FormedElectri~",
                        column: x => x.FormedElectricalComponentId,
                        principalTable: "FormedElectricalComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandElectricalComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StandId = table.Column<int>(type: "integer", nullable: false),
                    ElectricalComponentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandElectricalComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandElectricalComponents_FormedElectricalComponents_Electr~",
                        column: x => x.ElectricalComponentId,
                        principalTable: "FormedElectricalComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandElectricalComponents_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalEquipPurposes_FormedAdditionalEquipId",
                table: "AdditionalEquipPurposes",
                column: "FormedAdditionalEquipId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectricalPurposes_FormedElectricalComponentId",
                table: "ElectricalPurposes",
                column: "FormedElectricalComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_StandAdditionalEquips_AdditionalEquipId",
                table: "StandAdditionalEquips",
                column: "AdditionalEquipId");

            migrationBuilder.CreateIndex(
                name: "IX_StandAdditionalEquips_StandId",
                table: "StandAdditionalEquips",
                column: "StandId");

            migrationBuilder.CreateIndex(
                name: "IX_StandElectricalComponents_ElectricalComponentId",
                table: "StandElectricalComponents",
                column: "ElectricalComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_StandElectricalComponents_StandId",
                table: "StandElectricalComponents",
                column: "StandId");

            migrationBuilder.AddForeignKey(
                name: "FK_DrainagePurposes_FormedDrainages_FormedDrainageId",
                table: "DrainagePurposes",
                column: "FormedDrainageId",
                principalTable: "FormedDrainages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StandDrainages_FormedDrainages_DrainageId",
                table: "StandDrainages",
                column: "DrainageId",
                principalTable: "FormedDrainages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StandDrainages_Stands_StandId",
                table: "StandDrainages",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DrainagePurposes_FormedDrainages_FormedDrainageId",
                table: "DrainagePurposes");

            migrationBuilder.DropForeignKey(
                name: "FK_StandDrainages_FormedDrainages_DrainageId",
                table: "StandDrainages");

            migrationBuilder.DropForeignKey(
                name: "FK_StandDrainages_Stands_StandId",
                table: "StandDrainages");

            migrationBuilder.DropTable(
                name: "AdditionalEquipPurposes");

            migrationBuilder.DropTable(
                name: "ElectricalPurposes");

            migrationBuilder.DropTable(
                name: "StandAdditionalEquips");

            migrationBuilder.DropTable(
                name: "StandElectricalComponents");

            migrationBuilder.DropTable(
                name: "FormedAdditionalEquips");

            migrationBuilder.DropTable(
                name: "FormedElectricalComponents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StandDrainages",
                table: "StandDrainages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DrainagePurposes",
                table: "DrainagePurposes");

            migrationBuilder.RenameTable(
                name: "StandDrainages",
                newName: "StandDrainage");

            migrationBuilder.RenameTable(
                name: "DrainagePurposes",
                newName: "DrainagePurpose");

            migrationBuilder.RenameIndex(
                name: "IX_StandDrainages_StandId",
                table: "StandDrainage",
                newName: "IX_StandDrainage_StandId");

            migrationBuilder.RenameIndex(
                name: "IX_StandDrainages_DrainageId",
                table: "StandDrainage",
                newName: "IX_StandDrainage_DrainageId");

            migrationBuilder.RenameIndex(
                name: "IX_DrainagePurposes_FormedDrainageId",
                table: "DrainagePurpose",
                newName: "IX_DrainagePurpose_FormedDrainageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StandDrainage",
                table: "StandDrainage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DrainagePurpose",
                table: "DrainagePurpose",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DrainagePurpose_FormedDrainages_FormedDrainageId",
                table: "DrainagePurpose",
                column: "FormedDrainageId",
                principalTable: "FormedDrainages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StandDrainage_FormedDrainages_DrainageId",
                table: "StandDrainage",
                column: "DrainageId",
                principalTable: "FormedDrainages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StandDrainage_Stands_StandId",
                table: "StandDrainage",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
