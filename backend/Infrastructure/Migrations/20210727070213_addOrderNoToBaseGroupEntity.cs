using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addOrderNoToBaseGroupEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "order_no",
                table: "t_group_record",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "order_no",
                table: "t_group",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order_no",
                table: "t_group_record");

            migrationBuilder.DropColumn(
                name: "order_no",
                table: "t_group");
        }
    }
}
