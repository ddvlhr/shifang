using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addBoxMakingWorkShopAndRecycleBoxTypeToSystemSettingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "box_making_work_shop_id",
                table: "t_system_setting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "recycle_box_type_id",
                table: "t_system_setting",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "box_making_work_shop_id",
                table: "t_system_setting");

            migrationBuilder.DropColumn(
                name: "recycle_box_type_id",
                table: "t_system_setting");
        }
    }
}
