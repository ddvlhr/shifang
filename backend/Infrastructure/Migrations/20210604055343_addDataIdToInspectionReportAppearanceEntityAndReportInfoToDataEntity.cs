using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addDataIdToInspectionReportAppearanceEntityAndReportInfoToDataEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "data_id",
                table: "t_inspection_report_appearance",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "remark",
                table: "t_data_storage",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "result",
                table: "t_data_storage",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total",
                table: "t_data_storage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "remark",
                table: "t_data_inspection",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "result",
                table: "t_data_inspection",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total",
                table: "t_data_inspection",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "remark",
                table: "t_data_deliver",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "result",
                table: "t_data_deliver",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total",
                table: "t_data_deliver",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "remark",
                table: "t_data",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "result",
                table: "t_data",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total",
                table: "t_data",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_id",
                table: "t_inspection_report_appearance");

            migrationBuilder.DropColumn(
                name: "remark",
                table: "t_data_storage");

            migrationBuilder.DropColumn(
                name: "result",
                table: "t_data_storage");

            migrationBuilder.DropColumn(
                name: "total",
                table: "t_data_storage");

            migrationBuilder.DropColumn(
                name: "remark",
                table: "t_data_inspection");

            migrationBuilder.DropColumn(
                name: "result",
                table: "t_data_inspection");

            migrationBuilder.DropColumn(
                name: "total",
                table: "t_data_inspection");

            migrationBuilder.DropColumn(
                name: "remark",
                table: "t_data_deliver");

            migrationBuilder.DropColumn(
                name: "result",
                table: "t_data_deliver");

            migrationBuilder.DropColumn(
                name: "total",
                table: "t_data_deliver");

            migrationBuilder.DropColumn(
                name: "remark",
                table: "t_data");

            migrationBuilder.DropColumn(
                name: "result",
                table: "t_data");

            migrationBuilder.DropColumn(
                name: "total",
                table: "t_data");
        }
    }
}
