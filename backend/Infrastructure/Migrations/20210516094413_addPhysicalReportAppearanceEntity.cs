using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addPhysicalReportAppearanceEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_physical_report_appearance",
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
                    table.PrimaryKey("PK_t_physical_report_appearance", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_physical_report_appearance_t_indicator_indicator_id",
                        column: x => x.indicator_id,
                        principalTable: "t_indicator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_physical_report_appearance_t_physical_report_report_id",
                        column: x => x.report_id,
                        principalTable: "t_physical_report",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_physical_report_appearance_indicator_id",
                table: "t_physical_report_appearance",
                column: "indicator_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_physical_report_appearance_report_id",
                table: "t_physical_report_appearance",
                column: "report_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_physical_report_appearance");
        }
    }
}
