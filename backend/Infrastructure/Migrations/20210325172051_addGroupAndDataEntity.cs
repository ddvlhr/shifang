using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addGroupAndDataEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_group_deliver",
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
                    table.PrimaryKey("PK_t_group_deliver", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_group_deliver_t_machine_machine_id",
                        column: x => x.machine_id,
                        principalTable: "t_machine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_deliver_t_measure_type_measure_type_id",
                        column: x => x.measure_type_id,
                        principalTable: "t_measure_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_deliver_t_specification_specification_id",
                        column: x => x.specification_id,
                        principalTable: "t_specification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_deliver_t_turn_turn_id",
                        column: x => x.turn_id,
                        principalTable: "t_turn",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_group_inspection",
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
                    table.PrimaryKey("PK_t_group_inspection", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_group_inspection_t_machine_machine_id",
                        column: x => x.machine_id,
                        principalTable: "t_machine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_inspection_t_measure_type_measure_type_id",
                        column: x => x.measure_type_id,
                        principalTable: "t_measure_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_inspection_t_specification_specification_id",
                        column: x => x.specification_id,
                        principalTable: "t_specification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_inspection_t_turn_turn_id",
                        column: x => x.turn_id,
                        principalTable: "t_turn",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_group_storage",
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
                    table.PrimaryKey("PK_t_group_storage", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_group_storage_t_machine_machine_id",
                        column: x => x.machine_id,
                        principalTable: "t_machine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_storage_t_measure_type_measure_type_id",
                        column: x => x.measure_type_id,
                        principalTable: "t_measure_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_storage_t_specification_specification_id",
                        column: x => x.specification_id,
                        principalTable: "t_specification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_storage_t_turn_turn_id",
                        column: x => x.turn_id,
                        principalTable: "t_turn",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_data_inspection",
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
                    table.PrimaryKey("PK_t_data_inspection", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_data_inspection_t_group_inspection_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group_inspection",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_data_deliver",
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
                    table.PrimaryKey("PK_t_data_deliver", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_data_deliver_t_group_storage_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group_storage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_data_storage",
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
                    table.PrimaryKey("PK_t_data_storage", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_data_storage_t_group_storage_group_id",
                        column: x => x.group_id,
                        principalTable: "t_group_storage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_data_deliver_group_id",
                table: "t_data_deliver",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_data_inspection_group_id",
                table: "t_data_inspection",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_data_storage_group_id",
                table: "t_data_storage",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_deliver_machine_id",
                table: "t_group_deliver",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_deliver_measure_type_id",
                table: "t_group_deliver",
                column: "measure_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_deliver_specification_id",
                table: "t_group_deliver",
                column: "specification_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_deliver_turn_id",
                table: "t_group_deliver",
                column: "turn_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_inspection_machine_id",
                table: "t_group_inspection",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_inspection_measure_type_id",
                table: "t_group_inspection",
                column: "measure_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_inspection_specification_id",
                table: "t_group_inspection",
                column: "specification_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_inspection_turn_id",
                table: "t_group_inspection",
                column: "turn_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_storage_machine_id",
                table: "t_group_storage",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_storage_measure_type_id",
                table: "t_group_storage",
                column: "measure_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_storage_specification_id",
                table: "t_group_storage",
                column: "specification_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_storage_turn_id",
                table: "t_group_storage",
                column: "turn_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_data_deliver");

            migrationBuilder.DropTable(
                name: "t_data_inspection");

            migrationBuilder.DropTable(
                name: "t_data_storage");

            migrationBuilder.DropTable(
                name: "t_group_deliver");

            migrationBuilder.DropTable(
                name: "t_group_inspection");

            migrationBuilder.DropTable(
                name: "t_group_storage");
        }
    }
}
