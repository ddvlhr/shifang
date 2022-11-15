using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addUnqualifiedInfoToMeasureTypeIndicatorsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "unqualified_count",
                table: "t_measure_type_indicators",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "unqualified_operator",
                table: "t_measure_type_indicators",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "unqualified_count",
                table: "t_measure_type_indicators");

            migrationBuilder.DropColumn(
                name: "unqualified_operator",
                table: "t_measure_type_indicators");
        }
    }
}
