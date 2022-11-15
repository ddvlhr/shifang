using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addPhyRetDeductionToReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "phy_ret_deduction",
                table: "t_product_report",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "phy_ret_deduction",
                table: "t_physical_report",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "phy_ret_deduction",
                table: "t_inspection_report",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phy_ret_deduction",
                table: "t_product_report");

            migrationBuilder.DropColumn(
                name: "phy_ret_deduction",
                table: "t_physical_report");

            migrationBuilder.DropColumn(
                name: "phy_ret_deduction",
                table: "t_inspection_report");
        }
    }
}
