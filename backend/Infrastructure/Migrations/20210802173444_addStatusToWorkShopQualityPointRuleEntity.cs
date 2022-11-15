﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addStatusToWorkShopQualityPointRuleEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "t_work_shop_quality_point_rule",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "t_work_shop_quality_point_rule");
        }
    }
}
