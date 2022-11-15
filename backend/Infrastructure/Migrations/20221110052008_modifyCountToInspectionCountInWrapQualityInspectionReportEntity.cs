using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class modifyCountToInspectionCountInWrapQualityInspectionReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "count",
                table: "t_wrap_quality_inspection_report");

            migrationBuilder.AddColumn<string>(
                name: "inspection_count",
                table: "t_wrap_quality_inspection_report",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "专检次数")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inspection_count",
                table: "t_wrap_quality_inspection_report");

            migrationBuilder.AddColumn<int>(
                name: "count",
                table: "t_wrap_quality_inspection_report",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "专检次数");
        }
    }
}
