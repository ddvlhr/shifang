using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addCpkRulesToSpecificationAndMeasureTypeIndicatorsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cpk_rules",
                table: "t_specification",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "t_measure_type_indicators",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    measure_type_id = table.Column<int>(nullable: false),
                    indicator_id = table.Column<int>(nullable: false),
                    points = table.Column<int>(nullable: false),
                    deduction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_measure_type_indicators", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_measure_type_indicators_t_indicator_indicator_id",
                        column: x => x.indicator_id,
                        principalTable: "t_indicator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_measure_type_indicators_t_measure_type_measure_type_id",
                        column: x => x.measure_type_id,
                        principalTable: "t_measure_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_measure_type_indicators_indicator_id",
                table: "t_measure_type_indicators",
                column: "indicator_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_measure_type_indicators_measure_type_id",
                table: "t_measure_type_indicators",
                column: "measure_type_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_measure_type_indicators");

            migrationBuilder.DropColumn(
                name: "cpk_rules",
                table: "t_specification");
        }
    }
}
