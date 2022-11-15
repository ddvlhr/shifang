using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addWrapQualityInspectionReportAndDefectInfoEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_wrap_quality_inspection_report",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "时间"),
                    specification_id = table.Column<int>(type: "int", nullable: false, comment: "牌号 Id"),
                    team_id = table.Column<int>(type: "int", nullable: false, comment: "班组 Id"),
                    turn_id = table.Column<int>(type: "int", nullable: false, comment: "班次 Id"),
                    volume_pick_up_id = table.Column<int>(type: "int", nullable: false, comment: "卷接机 Id"),
                    packaging_machine_id = table.Column<int>(type: "int", nullable: false, comment: "包装机 Id"),
                    count = table.Column<int>(type: "int", nullable: false, comment: "专检次数"),
                    order_no = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "烟丝批号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    inspector = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "检验员")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    volume_pick_up_operator = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "卷接机操作工")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    packaging_machine_operator = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "包装机操作工")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    measure_group_ids = table.Column<string>(type: "text", nullable: true, comment: "测量组数据 Id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    total_points = table.Column<double>(type: "double", nullable: false, comment: "总扣分"),
                    volume_pick_up_points = table.Column<double>(type: "double", nullable: false, comment: "卷接机扣分"),
                    packaging_machine_points = table.Column<double>(type: "double", nullable: false, comment: "包装机扣分"),
                    result = table.Column<int>(type: "int", nullable: false, comment: "质量等级"),
                    batch_unqualified = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, comment: "批不合格项")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remark = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_wrap_quality_inspection_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_wrap_quality_inspection_report_t_packaging_machine_packagi~",
                        column: x => x.packaging_machine_id,
                        principalTable: "t_packaging_machine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_wrap_quality_inspection_report_t_specification_specificati~",
                        column: x => x.specification_id,
                        principalTable: "t_specification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_wrap_quality_inspection_report_t_team_team_id",
                        column: x => x.team_id,
                        principalTable: "t_team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_wrap_quality_inspection_report_t_turn_turn_id",
                        column: x => x.turn_id,
                        principalTable: "t_turn",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_wrap_quality_inspection_report_t_volume_pick_up_volume_pic~",
                        column: x => x.volume_pick_up_id,
                        principalTable: "t_volume_pick_up",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "卷包质量检验报表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_wrap_quality_inspection_report_defect",
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
                    table.PrimaryKey("PK_t_wrap_quality_inspection_report_defect", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_wrap_quality_inspection_report_defect_t_defect_defect_id",
                        column: x => x.defect_id,
                        principalTable: "t_defect",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_wrap_quality_inspection_report_defect_t_wrap_quality_inspe~",
                        column: x => x.report_id,
                        principalTable: "t_wrap_quality_inspection_report",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_quality_inspection_report_packaging_machine_id",
                table: "t_wrap_quality_inspection_report",
                column: "packaging_machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_quality_inspection_report_specification_id",
                table: "t_wrap_quality_inspection_report",
                column: "specification_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_quality_inspection_report_team_id",
                table: "t_wrap_quality_inspection_report",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_quality_inspection_report_turn_id",
                table: "t_wrap_quality_inspection_report",
                column: "turn_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_quality_inspection_report_volume_pick_up_id",
                table: "t_wrap_quality_inspection_report",
                column: "volume_pick_up_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_quality_inspection_report_defect_defect_id",
                table: "t_wrap_quality_inspection_report_defect",
                column: "defect_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_wrap_quality_inspection_report_defect_report_id",
                table: "t_wrap_quality_inspection_report_defect",
                column: "report_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_wrap_quality_inspection_report_defect");

            migrationBuilder.DropTable(
                name: "t_wrap_quality_inspection_report");
        }
    }
}
