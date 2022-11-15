using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addMaterialCheckReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_material_check_report",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    test_date = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "检测日期"),
                    specification_id = table.Column<int>(type: "int", nullable: false, comment: "牌号 Id"),
                    team_id = table.Column<int>(type: "int", nullable: false, comment: "班组 Id"),
                    turn_id = table.Column<int>(type: "int", nullable: false, comment: "班次 Id"),
                    machine_id = table.Column<int>(type: "int", nullable: false, comment: "机台 Id"),
                    measure_type_id = table.Column<int>(type: "int", nullable: false, comment: "测量类型 Id"),
                    group_id = table.Column<int>(type: "int", nullable: false, comment: "检测数据组数据 Id"),
                    originator = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "发起人")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    operating_user = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "检验员")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    qualified = table.Column<int>(type: "int", nullable: false, comment: "合格状态"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "流程状态"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_material_check_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_material_check_report_t_machine_machine_id",
                        column: x => x.machine_id,
                        principalTable: "t_machine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_material_check_report_t_measure_type_measure_type_id",
                        column: x => x.measure_type_id,
                        principalTable: "t_measure_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_material_check_report_t_specification_specification_id",
                        column: x => x.specification_id,
                        principalTable: "t_specification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_material_check_report_t_team_team_id",
                        column: x => x.team_id,
                        principalTable: "t_team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_material_check_report_t_turn_turn_id",
                        column: x => x.turn_id,
                        principalTable: "t_turn",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "物资申检报表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_material_check_report_machine_id",
                table: "t_material_check_report",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_material_check_report_measure_type_id",
                table: "t_material_check_report",
                column: "measure_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_material_check_report_specification_id",
                table: "t_material_check_report",
                column: "specification_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_material_check_report_team_id",
                table: "t_material_check_report",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_material_check_report_turn_id",
                table: "t_material_check_report",
                column: "turn_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_material_check_report");
        }
    }
}
