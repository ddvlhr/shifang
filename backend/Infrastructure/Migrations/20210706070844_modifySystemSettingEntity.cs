using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class modifySystemSettingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "factory_type_id",
                table: "t_system_setting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "filter_type_id",
                table: "t_system_setting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "inspection_type_id",
                table: "t_system_setting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "material_type_id",
                table: "t_system_setting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "mysql_server_name",
                table: "t_system_setting",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "production_type_id",
                table: "t_system_setting",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "factory_type_id",
                table: "t_system_setting");

            migrationBuilder.DropColumn(
                name: "filter_type_id",
                table: "t_system_setting");

            migrationBuilder.DropColumn(
                name: "inspection_type_id",
                table: "t_system_setting");

            migrationBuilder.DropColumn(
                name: "material_type_id",
                table: "t_system_setting");

            migrationBuilder.DropColumn(
                name: "mysql_server_name",
                table: "t_system_setting");

            migrationBuilder.DropColumn(
                name: "production_type_id",
                table: "t_system_setting");
        }
    }
}
