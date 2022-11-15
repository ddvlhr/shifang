using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class removeManufacturerFForeignKeyInMaterialReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_material_report_t_manufacturer_manufacturer_id",
                table: "t_material_report");

            migrationBuilder.DropIndex(
                name: "IX_t_material_report_manufacturer_id",
                table: "t_material_report");

            migrationBuilder.AddColumn<string>(
                name: "manufacturer_name",
                table: "t_material_report",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "manufacturer_name",
                table: "t_material_report");

            migrationBuilder.CreateIndex(
                name: "IX_t_material_report_manufacturer_id",
                table: "t_material_report",
                column: "manufacturer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_material_report_t_manufacturer_manufacturer_id",
                table: "t_material_report",
                column: "manufacturer_id",
                principalTable: "t_manufacturer",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
