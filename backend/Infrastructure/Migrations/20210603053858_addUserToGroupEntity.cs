using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addUserToGroupEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "t_group_storage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "t_group_storage",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "t_group_inspection",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "t_group_inspection",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "t_group_deliver",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "t_group_deliver",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "t_group",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "t_group",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_id",
                table: "t_group_storage");

            migrationBuilder.DropColumn(
                name: "user_name",
                table: "t_group_storage");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "t_group_inspection");

            migrationBuilder.DropColumn(
                name: "user_name",
                table: "t_group_inspection");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "t_group_deliver");

            migrationBuilder.DropColumn(
                name: "user_name",
                table: "t_group_deliver");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "t_group");

            migrationBuilder.DropColumn(
                name: "user_name",
                table: "t_group");
        }
    }
}
