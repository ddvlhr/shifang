using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addProductOrderNoToSpecficationTypeEntityAndAddFromRecordsToBaseGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "product_order_no",
                table: "t_specification_type",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "from_records",
                table: "t_group_record",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "from_records",
                table: "t_group",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "product_order_no",
                table: "t_specification_type");

            migrationBuilder.DropColumn(
                name: "from_records",
                table: "t_group_record");

            migrationBuilder.DropColumn(
                name: "from_records",
                table: "t_group");
        }
    }
}
