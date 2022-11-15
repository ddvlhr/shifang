using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addDefectEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_defect_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_defect_events", x => x.id);
                },
                comment: "缺陷类别小项")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_defect_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_defect_type", x => x.id);
                },
                comment: "缺陷类别")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_defect",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    defect_type_id = table.Column<int>(type: "int", nullable: false, comment: "缺陷类别 Id"),
                    defect_events_id = table.Column<int>(type: "int", nullable: false, comment: "缺陷类别小项 Id"),
                    defect_short_name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, comment: "缺陷简称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    defect_code = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "缺陷代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "text", nullable: true, comment: "缺陷描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    defect_category = table.Column<int>(type: "int", nullable: false, comment: "缺陷分类[A, B, C, D]"),
                    score = table.Column<double>(type: "double", nullable: false, comment: "扣分值"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_defect", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_defect_t_defect_type_defect_type_id",
                        column: x => x.defect_type_id,
                        principalTable: "t_defect_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "缺陷表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_defect_defect_type_id",
                table: "t_defect",
                column: "defect_type_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_defect");

            migrationBuilder.DropTable(
                name: "t_defect_events");

            migrationBuilder.DropTable(
                name: "t_defect_type");
        }
    }
}
