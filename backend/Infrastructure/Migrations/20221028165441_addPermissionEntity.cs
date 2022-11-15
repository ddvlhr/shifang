using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addPermissionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "权限名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    permission_type = table.Column<int>(type: "int", nullable: false, comment: "权限类型[1 菜单, 2 按钮]"),
                    level = table.Column<int>(type: "int", nullable: false, comment: "菜单层级"),
                    path = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true, comment: "菜单路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    icon = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true, comment: "图标")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    component = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, comment: "组件路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    function_name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true, comment: "按钮方法名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    button_type = table.Column<int>(type: "int", nullable: false, comment: "按钮类型"),
                    root = table.Column<int>(type: "int", nullable: false, comment: "菜单 Id"),
                    button_position = table.Column<int>(type: "int", nullable: false, comment: "按钮位置[1 顶部, 2 行内]"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "状态"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_permission", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_permission");
        }
    }
}
