﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addSpecificationTypeRuleEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_specification_type_rule",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    specification_type_id = table.Column<int>(nullable: false),
                    rules = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_specification_type_rule", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_specification_type_rule_t_specification_type_specification~",
                        column: x => x.specification_type_id,
                        principalTable: "t_specification_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_specification_type_rule_specification_type_id",
                table: "t_specification_type_rule",
                column: "specification_type_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_specification_type_rule");
        }
    }
}
