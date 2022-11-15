using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class InitialFunctionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_function",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(maxLength: 64, nullable: true),
                    function_name = table.Column<string>(maxLength: 64, nullable: true),
                    type = table.Column<string>(maxLength: 64, nullable: true),
                    menu_id = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_function", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_function_t_menu_menu_id",
                        column: x => x.menu_id,
                        principalTable: "t_menu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_function_menu_id",
                table: "t_function",
                column: "menu_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_function");
        }
    }
}
