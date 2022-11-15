using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addCvToMeasureTypeIndicators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cv_deduction",
                table: "t_measure_type_indicators",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cv_points",
                table: "t_measure_type_indicators",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cv_deduction",
                table: "t_measure_type_indicators");

            migrationBuilder.DropColumn(
                name: "cv_points",
                table: "t_measure_type_indicators");
        }
    }
}
