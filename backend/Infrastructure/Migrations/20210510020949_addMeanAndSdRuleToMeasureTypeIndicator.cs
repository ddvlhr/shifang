using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addMeanAndSdRuleToMeasureTypeIndicator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "mean_deduction",
                table: "t_measure_type_indicators",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "mean_points",
                table: "t_measure_type_indicators",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sd_deduction",
                table: "t_measure_type_indicators",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sd_points",
                table: "t_measure_type_indicators",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mean_deduction",
                table: "t_measure_type_indicators");

            migrationBuilder.DropColumn(
                name: "mean_points",
                table: "t_measure_type_indicators");

            migrationBuilder.DropColumn(
                name: "sd_deduction",
                table: "t_measure_type_indicators");

            migrationBuilder.DropColumn(
                name: "sd_points",
                table: "t_measure_type_indicators");
        }
    }
}
