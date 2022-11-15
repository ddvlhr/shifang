using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addManualInspectionReportEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_manual_inspection_report",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "时间"),
                    specification_id = table.Column<int>(type: "int", nullable: false, comment: "牌号 Id"),
                    operation = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "操作工")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    count = table.Column<int>(type: "int", nullable: false, comment: "数量"),
                    result = table.Column<int>(type: "int", nullable: false, comment: "判定结果"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_manual_inspection_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_manual_inspection_report_t_specification_specification_id",
                        column: x => x.specification_id,
                        principalTable: "t_specification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_manual_inspection_report_defect",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    report_id = table.Column<int>(type: "int", nullable: false, comment: "手工检验报表 Id"),
                    defect_id = table.Column<int>(type: "int", nullable: false, comment: "缺陷 Id"),
                    count = table.Column<int>(type: "int", nullable: false, comment: "数量"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_manual_inspection_report_defect", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_manual_inspection_report_defect_t_defect_defect_id",
                        column: x => x.defect_id,
                        principalTable: "t_defect",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_manual_inspection_report_defect_t_manual_inspection_report~",
                        column: x => x.report_id,
                        principalTable: "t_manual_inspection_report",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "手工检验报表缺陷项")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_manual_inspection_report_specification_id",
                table: "t_manual_inspection_report",
                column: "specification_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_manual_inspection_report_defect_defect_id",
                table: "t_manual_inspection_report_defect",
                column: "defect_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_manual_inspection_report_defect_report_id",
                table: "t_manual_inspection_report_defect",
                column: "report_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_manual_inspection_report_defect");

            migrationBuilder.DropTable(
                name: "t_manual_inspection_report");
        }
    }
}
