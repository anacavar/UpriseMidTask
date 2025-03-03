using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UpriseMidTask.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionData_SolarPlants_SolarPowerPlantId",
                table: "ProductionData");

            migrationBuilder.RenameColumn(
                name: "SolarPowerPlantId",
                table: "ProductionData",
                newName: "SolarPlantId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductionData_SolarPowerPlantId",
                table: "ProductionData",
                newName: "IX_ProductionData_SolarPlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionData_SolarPlants_SolarPlantId",
                table: "ProductionData",
                column: "SolarPlantId",
                principalTable: "SolarPlants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionData_SolarPlants_SolarPlantId",
                table: "ProductionData");

            migrationBuilder.RenameColumn(
                name: "SolarPlantId",
                table: "ProductionData",
                newName: "SolarPowerPlantId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductionData_SolarPlantId",
                table: "ProductionData",
                newName: "IX_ProductionData_SolarPowerPlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionData_SolarPlants_SolarPowerPlantId",
                table: "ProductionData",
                column: "SolarPowerPlantId",
                principalTable: "SolarPlants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
