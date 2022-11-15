using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addProcessDisciplinReportEntityAndDepartmentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_department",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_department", x => x.id);
                },
                comment: "部门")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_process_discipline_report",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "检测时间"),
                    department_id = table.Column<int>(type: "int", nullable: false, comment: "涉及部门"),
                    description = table.Column<string>(type: "text", nullable: true, comment: "现象描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reward = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true, comment: "奖励情况")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    punishment = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true, comment: "处罚情况")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_process_discipline_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_process_discipline_report_t_department_department_id",
                        column: x => x.department_id,
                        principalTable: "t_department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "工艺纪律执行情况")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_process_discipline_report_department_id",
                table: "t_process_discipline_report",
                column: "department_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_process_discipline_report");

            migrationBuilder.DropTable(
                name: "t_department");
        }
    }
}
