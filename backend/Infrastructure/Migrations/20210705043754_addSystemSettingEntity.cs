using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addSystemSettingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_system_setting",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    CanSeeOtherData = table.Column<bool>(nullable: false),
                    AdminId = table.Column<int>(nullable: false),
                    ChemicalTypeId = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: false),
                    Circle = table.Column<int>(nullable: false),
                    Oval = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    Resistance = table.Column<int>(nullable: false),
                    Hardness = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_system_setting", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_system_setting");
        }
    }
}
