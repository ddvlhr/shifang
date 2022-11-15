using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addBaseGroupAndDataEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_group",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    begin_time = table.Column<DateTime>(nullable: false),
                    end_time = table.Column<DateTime>(nullable: false),
                    production_time = table.Column<DateTime>(nullable: true),
                    deliver_time = table.Column<DateTime>(nullable: true),
                    specification_id = table.Column<int>(nullable: false),
                    turn_id = table.Column<int>(nullable: false),
                    machine_id = table.Column<int>(nullable: false),
                    measure_type_id = table.Column<int>(nullable: false),
                    instance = table.Column<string>(maxLength: 64, nullable: true),
                    pickup_way = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_group", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_group_t_machine_machine_id",
                        column: x => x.machine_id,
                        principalTable: "t_machine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_t_measure_type_measure_type_id",
                        column: x => x.measure_type_id,
                        principalTable: "t_measure_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_t_specification_specification_id",
                        column: x => x.specification_id,
                        principalTable: "t_specification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_t_turn_turn_id",
                        column: x => x.turn_id,
                        principalTable: "t_turn",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_data",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    group_id = table.Column<int>(nullable: false),
                    test_time = table.Column<DateTime>(nullable: false),
                    data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_data", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_data_t_group_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_data_group_id",
                table: "t_data",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_machine_id",
                table: "t_group",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_measure_type_id",
                table: "t_group",
                column: "measure_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_specification_id",
                table: "t_group",
                column: "specification_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_turn_id",
                table: "t_group",
                column: "turn_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_data");

            migrationBuilder.DropTable(
                name: "t_group");
        }
    }
}
