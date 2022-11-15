using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addTestReportEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_test_report",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    water = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    humidity = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    temperature = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phy_ret = table.Column<int>(type: "int", nullable: false),
                    phy_ret_des = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phy_ret_deduction = table.Column<int>(type: "int", nullable: false),
                    remark = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    final_ret = table.Column<int>(type: "int", nullable: false),
                    total = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_test_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_test_report_t_group_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_test_report_appearance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    report_id = table.Column<int>(type: "int", nullable: false),
                    indicator_id = table.Column<int>(type: "int", nullable: false),
                    frequency = table.Column<int>(type: "int", nullable: false),
                    sub_score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_test_report_appearance", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_test_report_appearance_t_indicator_indicator_id",
                        column: x => x.indicator_id,
                        principalTable: "t_indicator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_test_report_group_id",
                table: "t_test_report",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_test_report_appearance_indicator_id",
                table: "t_test_report_appearance",
                column: "indicator_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_test_report");

            migrationBuilder.DropTable(
                name: "t_test_report_appearance");
        }
    }
}
