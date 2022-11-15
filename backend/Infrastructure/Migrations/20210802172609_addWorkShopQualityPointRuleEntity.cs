using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addWorkShopQualityPointRuleEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_work_shop_quality_point_rule",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    work_shop_id = table.Column<int>(nullable: false),
                    specification_type_id = table.Column<int>(nullable: false),
                    physical_points_percent = table.Column<int>(nullable: false),
                    physical_all_percent = table.Column<int>(nullable: false),
                    inspection_points_percent = table.Column<int>(nullable: false),
                    inspection_all_percent = table.Column<int>(nullable: false),
                    product_points_percent = table.Column<int>(nullable: false),
                    product_all_percent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_work_shop_quality_point_rule", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_work_shop_quality_point_rule_t_specification_type_specific~",
                        column: x => x.specification_type_id,
                        principalTable: "t_specification_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_work_shop_quality_point_rule_t_work_shop_work_shop_id",
                        column: x => x.work_shop_id,
                        principalTable: "t_work_shop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_work_shop_quality_point_rule_specification_type_id",
                table: "t_work_shop_quality_point_rule",
                column: "specification_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_work_shop_quality_point_rule_work_shop_id",
                table: "t_work_shop_quality_point_rule",
                column: "work_shop_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_work_shop_quality_point_rule");
        }
    }
}
