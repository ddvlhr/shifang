using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addWrapProcessInspectionReportAndDefectEntites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_wrap_process_inspection_report",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "检测时间"),
                    specification_id = table.Column<int>(type: "int", nullable: false, comment: "牌号ID"),
                    turn_id = table.Column<int>(type: "int", nullable: false, comment: "班次ID"),
                    machine_id = table.Column<int>(type: "int", nullable: false, comment: "机台ID"),
                    batch_number = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "批号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    operator_name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "操作员")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    score = table.Column<double>(type: "double", nullable: false, comment: "扣分"),
                    result = table.Column<int>(type: "int", nullable: false, comment: "判定结果"),
                    inspector = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "检验员")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    weight_upper = table.Column<int>(type: "int", nullable: false, comment: "重量偏大支数"),
                    weight_lower = table.Column<int>(type: "int", nullable: false, comment: "重量偏小支数"),
                    resistance_upper = table.Column<int>(type: "int", nullable: false, comment: "吸阻偏大支数"),
                    resistance_lower = table.Column<int>(type: "int", nullable: false, comment: "吸阻偏小支数"),
                    other_indicators = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true, comment: "其他指标超标")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    other_count = table.Column<int>(type: "int", nullable: false, comment: "其他指标超标支数"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_wrap_process_inspection_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_wrap_process_inspection_report_t_machine_machine_id",
                        column: x => x.machine_id,
                        principalTable: "t_machine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_wrap_process_inspection_report_t_specification_specificati~",
                        column: x => x.specification_id,
                        principalTable: "t_specification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_wrap_process_inspection_report_t_turn_turn_id",
                        column: x => x.turn_id,
                        principalTable: "t_turn",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_wrap_process_inspection_report_defect",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    report_id = table.Column<int>(type: "int", nullable: false, comment: "卷包质量检验报表 Id"),
                    defect_id = table.Column<int>(type: "int", nullable: false, comment: "缺陷 Id"),
                    count = table.Column<int>(type: "int", nullable: false, comment: "数量"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_wrap_process_inspection_report_defect", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_wrap_process_inspection_report_defect_t_defect_defect_id",
                        column: x => x.defect_id,
                        principalTable: "t_defect",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_wrap_process_inspection_report_defect_t_wrap_process_inspe~",
                        column: x => x.report_id,
                        principalTable: "t_wrap_process_inspection_report",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "卷制过程检验报告缺陷表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_process_inspection_report_machine_id",
                table: "t_wrap_process_inspection_report",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_process_inspection_report_specification_id",
                table: "t_wrap_process_inspection_report",
                column: "specification_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_process_inspection_report_turn_id",
                table: "t_wrap_process_inspection_report",
                column: "turn_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_process_inspection_report_defect_defect_id",
                table: "t_wrap_process_inspection_report_defect",
                column: "defect_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_process_inspection_report_defect_report_id",
                table: "t_wrap_process_inspection_report_defect",
                column: "report_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_wrap_process_inspection_report_defect");

            migrationBuilder.DropTable(
                name: "t_wrap_process_inspection_report");
        }
    }
}
