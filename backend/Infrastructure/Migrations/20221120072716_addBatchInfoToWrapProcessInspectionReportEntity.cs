using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addBatchInfoToWrapProcessInspectionReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "batch_unqualified",
                table: "t_wrap_process_inspection_report",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true,
                comment: "批不合格项")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "remark",
                table: "t_wrap_process_inspection_report",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true,
                comment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "batch_unqualified",
                table: "t_wrap_process_inspection_report");

            migrationBuilder.DropColumn(
                name: "remark",
                table: "t_wrap_process_inspection_report");
        }
    }
}
