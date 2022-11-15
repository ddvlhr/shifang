using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addTemperatureAndHumidityToMaterialReportEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "humidity",
                table: "t_material_report",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "temperature",
                table: "t_material_report",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "humidity",
                table: "t_material_report");

            migrationBuilder.DropColumn(
                name: "temperature",
                table: "t_material_report");
        }
    }
}
