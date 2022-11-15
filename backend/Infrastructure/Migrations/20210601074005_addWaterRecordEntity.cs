using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addWaterRecordEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_water_record",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    group_id = table.Column<int>(nullable: false),
                    before = table.Column<double>(nullable: false),
                    after = table.Column<double>(nullable: false),
                    water = table.Column<double>(nullable: false),
                    data_id = table.Column<int>(nullable: false),
                    data_test_time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_water_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_water_record_t_group_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_water_record_group_id",
                table: "t_water_record",
                column: "group_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_water_record");
        }
    }
}
