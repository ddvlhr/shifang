using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addInspectionAndProductReportEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_inspection_report",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    group_id = table.Column<int>(nullable: false),
                    water = table.Column<string>(maxLength: 64, nullable: true),
                    humidity = table.Column<string>(maxLength: 64, nullable: true),
                    temperature = table.Column<string>(maxLength: 64, nullable: true),
                    phy_ret = table.Column<int>(nullable: false),
                    phy_ret_des = table.Column<string>(maxLength: 256, nullable: true),
                    remark = table.Column<string>(maxLength: 128, nullable: true),
                    final_ret = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_inspection_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_inspection_report_t_group_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_inspection_report_appearance",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    report_id = table.Column<int>(nullable: false),
                    indicator_id = table.Column<int>(nullable: false),
                    frequency = table.Column<int>(nullable: false),
                    sub_score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_inspection_report_appearance", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_inspection_report_appearance_t_indicator_indicator_id",
                        column: x => x.indicator_id,
                        principalTable: "t_indicator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_product_report",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    group_id = table.Column<int>(nullable: false),
                    water = table.Column<string>(maxLength: 64, nullable: true),
                    humidity = table.Column<string>(maxLength: 64, nullable: true),
                    temperature = table.Column<string>(maxLength: 64, nullable: true),
                    phy_ret = table.Column<int>(nullable: false),
                    phy_ret_des = table.Column<string>(maxLength: 256, nullable: true),
                    remark = table.Column<string>(maxLength: 128, nullable: true),
                    final_ret = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_product_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_product_report_t_group_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_product_report_appearance",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    report_id = table.Column<int>(nullable: false),
                    indicator_id = table.Column<int>(nullable: false),
                    frequency = table.Column<int>(nullable: false),
                    sub_score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_product_report_appearance", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_product_report_appearance_t_indicator_indicator_id",
                        column: x => x.indicator_id,
                        principalTable: "t_indicator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_inspection_report_group_id",
                table: "t_inspection_report",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_inspection_report_appearance_indicator_id",
                table: "t_inspection_report_appearance",
                column: "indicator_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_report_group_id",
                table: "t_product_report",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_report_appearance_indicator_id",
                table: "t_product_report_appearance",
                column: "indicator_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_inspection_report");

            migrationBuilder.DropTable(
                name: "t_inspection_report_appearance");

            migrationBuilder.DropTable(
                name: "t_product_report");

            migrationBuilder.DropTable(
                name: "t_product_report_appearance");
        }
    }
}
