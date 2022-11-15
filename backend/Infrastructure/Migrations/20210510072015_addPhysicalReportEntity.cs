using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addPhysicalReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_physical_report",
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
                    table.PrimaryKey("PK_t_physical_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_physical_report_t_group_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_physical_report_group_id",
                table: "t_physical_report",
                column: "group_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_physical_report");
        }
    }
}
