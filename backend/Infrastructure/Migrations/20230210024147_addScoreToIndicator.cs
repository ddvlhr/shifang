using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addScoreToIndicator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<string>(
            //     name: "batch_number",
            //     table: "t_product_report",
            //     type: "varchar(64)",
            //     maxLength: 64,
            //     nullable: true,
            //     comment: "烟支批号")
            //     .Annotation("MySql:CharSet", "utf8mb4");
            //
            // migrationBuilder.AddColumn<double>(
            //     name: "end_tribe_count",
            //     table: "t_product_report",
            //     type: "double",
            //     nullable: false,
            //     defaultValue: 0.0,
            //     comment: "端部落丝量");
            //
            // migrationBuilder.AddColumn<int>(
            //     name: "flame_out_count",
            //     table: "t_product_report",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0,
            //     comment: "熄火支数");
            //
            // migrationBuilder.AddColumn<string>(
            //     name: "inspector",
            //     table: "t_product_report",
            //     type: "varchar(64)",
            //     maxLength: 64,
            //     nullable: true,
            //     comment: "检验员")
            //     .Annotation("MySql:CharSet", "utf8mb4");
            //
            // migrationBuilder.AddColumn<double>(
            //     name: "moisture_rate",
            //     table: "t_product_report",
            //     type: "double",
            //     nullable: false,
            //     defaultValue: 0.0,
            //     comment: "含末率");

            migrationBuilder.AddColumn<double>(
                name: "score",
                table: "t_indicator",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            // migrationBuilder.CreateTable(
            //     name: "t_product_report_defect",
            //     columns: table => new
            //     {
            //         id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //         report_id = table.Column<int>(type: "int", nullable: false, comment: "成品质量检验报表 Id"),
            //         defect_id = table.Column<int>(type: "int", nullable: false, comment: "缺陷 Id"),
            //         count = table.Column<int>(type: "int", nullable: false, comment: "数量"),
            //         created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_t_product_report_defect", x => x.id);
            //         table.ForeignKey(
            //             name: "FK_t_product_report_defect_t_defect_defect_id",
            //             column: x => x.defect_id,
            //             principalTable: "t_defect",
            //             principalColumn: "id",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_t_product_report_defect_t_product_report_report_id",
            //             column: x => x.report_id,
            //             principalTable: "t_product_report",
            //             principalColumn: "id",
            //             onDelete: ReferentialAction.Cascade);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_t_product_report_defect_defect_id",
            //     table: "t_product_report_defect",
            //     column: "defect_id");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_t_product_report_defect_report_id",
            //     table: "t_product_report_defect",
            //     column: "report_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropTable(
            //     name: "t_product_report_defect");
            //
            // migrationBuilder.DropColumn(
            //     name: "batch_number",
            //     table: "t_product_report");
            //
            // migrationBuilder.DropColumn(
            //     name: "end_tribe_count",
            //     table: "t_product_report");
            //
            // migrationBuilder.DropColumn(
            //     name: "flame_out_count",
            //     table: "t_product_report");
            //
            // migrationBuilder.DropColumn(
            //     name: "inspector",
            //     table: "t_product_report");
            //
            // migrationBuilder.DropColumn(
            //     name: "moisture_rate",
            //     table: "t_product_report");

            migrationBuilder.DropColumn(
                name: "score",
                table: "t_indicator");
        }
    }
}
