using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addGroupForeignKeyToMaterialReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "group_id",
                table: "t_material_report",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_t_material_report_group_id",
                table: "t_material_report",
                column: "group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_material_report_t_group_group_id",
                table: "t_material_report",
                column: "group_id",
                principalTable: "t_group",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_material_report_t_group_group_id",
                table: "t_material_report");

            migrationBuilder.DropIndex(
                name: "IX_t_material_report_group_id",
                table: "t_material_report");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "t_material_report");
        }
    }
}
