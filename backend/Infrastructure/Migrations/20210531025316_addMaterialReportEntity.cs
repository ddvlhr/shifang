using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addMaterialReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_material_report",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    test_date = table.Column<DateTime>(nullable: false),
                    order_no = table.Column<string>(maxLength: 128, nullable: true),
                    manufacturer_id = table.Column<int>(nullable: false),
                    specification_id = table.Column<int>(nullable: false),
                    sample_place = table.Column<string>(maxLength: 64, nullable: true),
                    sample_count = table.Column<string>(maxLength: 64, nullable: true),
                    unit = table.Column<string>(maxLength: 64, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    result = table.Column<string>(maxLength: 256, nullable: true),
                    checker = table.Column<int>(nullable: false),
                    report_ret = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_material_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_material_report_t_user_checker",
                        column: x => x.checker,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_material_report_t_manufacturer_manufacturer_id",
                        column: x => x.manufacturer_id,
                        principalTable: "t_manufacturer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_material_report_t_specification_type_specification_id",
                        column: x => x.specification_id,
                        principalTable: "t_specification_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_material_report_checker",
                table: "t_material_report",
                column: "checker");

            migrationBuilder.CreateIndex(
                name: "IX_t_material_report_manufacturer_id",
                table: "t_material_report",
                column: "manufacturer_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_material_report_specification_id",
                table: "t_material_report",
                column: "specification_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_material_report");
        }
    }
}
