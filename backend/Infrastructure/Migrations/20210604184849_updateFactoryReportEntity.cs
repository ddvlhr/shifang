using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class updateFactoryReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "report_ret",
                table: "t_factory_report",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "specification_id",
                table: "t_factory_report",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_t_factory_report_specification_id",
                table: "t_factory_report",
                column: "specification_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_factory_report_t_specification_specification_id",
                table: "t_factory_report",
                column: "specification_id",
                principalTable: "t_specification",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_factory_report_t_specification_specification_id",
                table: "t_factory_report");

            migrationBuilder.DropIndex(
                name: "IX_t_factory_report_specification_id",
                table: "t_factory_report");

            migrationBuilder.DropColumn(
                name: "report_ret",
                table: "t_factory_report");

            migrationBuilder.DropColumn(
                name: "specification_id",
                table: "t_factory_report");
        }
    }
}
