using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addSpecificationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_specification",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(maxLength: 128, nullable: true),
                    order_no = table.Column<string>(maxLength: 128, nullable: true),
                    specification_type_id = table.Column<int>(nullable: false),
                    remark = table.Column<string>(maxLength: 64, nullable: true),
                    single_rules = table.Column<string>(type: "text", nullable: true),
                    mean_rules = table.Column<string>(type: "text", nullable: true),
                    sd_rules = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_specification", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_specification_t_specification_type_specification_type_id",
                        column: x => x.specification_type_id,
                        principalTable: "t_specification_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_specification_specification_type_id",
                table: "t_specification",
                column: "specification_type_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_specification");
        }
    }
}
