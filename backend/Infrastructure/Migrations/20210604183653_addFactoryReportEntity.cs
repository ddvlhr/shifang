using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addFactoryReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_factory_report",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    test_date = table.Column<DateTime>(nullable: false),
                    group_id = table.Column<int>(nullable: false),
                    order_no = table.Column<string>(maxLength: 128, nullable: true),
                    manufacturer_place = table.Column<string>(maxLength: 128, nullable: true),
                    test_method = table.Column<string>(maxLength: 64, nullable: true),
                    result = table.Column<string>(maxLength: 256, nullable: true),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_factory_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_factory_report_t_group_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_factory_report_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_factory_report_group_id",
                table: "t_factory_report",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_factory_report_user_id",
                table: "t_factory_report",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_factory_report");
        }
    }
}
