using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class addCalibrationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_calibration",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "标定验证时间"),
                    instance = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "仪器名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    equipment_type = table.Column<int>(type: "int", nullable: false, comment: "测试台类型"),
                    operation = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "操作")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "单元")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit_type = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "单位")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    result_code = table.Column<int>(type: "int", nullable: false, comment: "结果"),
                    description = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, comment: "描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    temperature = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "温度")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    humidity = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, comment: "湿度")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_calibration", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_calibration");
        }
    }
}
