using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addUploadRecordEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "machine_id",
                table: "t_group",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "t_data_record",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    group_id = table.Column<int>(nullable: false),
                    test_time = table.Column<DateTime>(nullable: false),
                    data = table.Column<string>(type: "text", nullable: true),
                    total = table.Column<int>(nullable: false),
                    result = table.Column<string>(maxLength: 64, nullable: true),
                    remark = table.Column<string>(maxLength: 64, nullable: true),
                    weight = table.Column<double>(nullable: true),
                    circle = table.Column<double>(nullable: true),
                    oval = table.Column<double>(nullable: true),
                    length = table.Column<double>(nullable: true),
                    resistance = table.Column<double>(nullable: true),
                    resistance_open = table.Column<double>(nullable: true),
                    hardness = table.Column<double>(nullable: true),
                    ventilation_filter = table.Column<double>(nullable: true),
                    ventilation_cigarette = table.Column<double>(nullable: true),
                    ventilation_total = table.Column<double>(nullable: true),
                    count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_data_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_group_record",
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
                    machine_model_id = table.Column<int>(nullable: false),
                    instance = table.Column<string>(maxLength: 64, nullable: true),
                    pickup_way = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    user_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_group_record", x => x.id);
                    // table.ForeignKey(
                    //     name: "FK_t_group_record_t_machine_machine_id",
                    //     column: x => x.machine_id,
                    //     principalTable: "t_machine",
                    //     principalColumn: "id",
                    //     onDelete: ReferentialAction.Cascade);
                    // table.ForeignKey(
                    //     name: "FK_t_group_record_t_machine_model_machine_model_id",
                    //     column: x => x.machine_model_id,
                    //     principalTable: "t_machine_model",
                    //     principalColumn: "id",
                    //     onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_record_t_measure_type_measure_type_id",
                        column: x => x.measure_type_id,
                        principalTable: "t_measure_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_record_t_specification_specification_id",
                        column: x => x.specification_id,
                        principalTable: "t_specification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_group_record_t_turn_turn_id",
                        column: x => x.turn_id,
                        principalTable: "t_turn",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_group_machine_id",
                table: "t_group",
                column: "machine_id");

            // migrationBuilder.CreateIndex(
            //     name: "IX_t_group_record_machine_id",
            //     table: "t_group_record",
            //     column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_record_machine_model_id",
                table: "t_group_record",
                column: "machine_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_record_measure_type_id",
                table: "t_group_record",
                column: "measure_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_record_specification_id",
                table: "t_group_record",
                column: "specification_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_record_turn_id",
                table: "t_group_record",
                column: "turn_id");

            migrationBuilder.DropForeignKey(
                name: "FK_t_group_t_machine_model_machine_model_id",
                table: "t_group");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_t_group_t_machine_machine_id",
            //     table: "t_group",
            //     column: "machine_id",
            //     principalTable: "t_machine",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_group_t_machine_machine_id",
                table: "t_group");

            migrationBuilder.DropTable(
                name: "t_data_record");

            migrationBuilder.DropTable(
                name: "t_group_record");

            migrationBuilder.DropIndex(
                name: "IX_t_group_machine_id",
                table: "t_group");

            migrationBuilder.DropColumn(
                name: "machine_id",
                table: "t_group");

            migrationBuilder.CreateTable(
                name: "t_group_deliver",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    begin_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deliver_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    instance = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    machine_model_id = table.Column<int>(type: "int", nullable: false),
                    measure_type_id = table.Column<int>(type: "int", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    pickup_way = table.Column<int>(type: "int", nullable: false),
                    production_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    specification_id = table.Column<int>(type: "int", nullable: false),
                    turn_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    user_name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_group_deliver", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_group_deliver_t_machine_model_machine_model_id",
                        column: x => x.machine_model_id,
                        principalTable: "t_machine_model",
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
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    begin_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deliver_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    instance = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    machine_model_id = table.Column<int>(type: "int", nullable: false),
                    measure_type_id = table.Column<int>(type: "int", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    pickup_way = table.Column<int>(type: "int", nullable: false),
                    production_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    specification_id = table.Column<int>(type: "int", nullable: false),
                    turn_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    user_name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_group_inspection", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_group_inspection_t_machine_model_machine_model_id",
                        column: x => x.machine_model_id,
                        principalTable: "t_machine_model",
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
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    begin_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deliver_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    instance = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    machine_model_id = table.Column<int>(type: "int", nullable: false),
                    measure_type_id = table.Column<int>(type: "int", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    pickup_way = table.Column<int>(type: "int", nullable: false),
                    production_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    specification_id = table.Column<int>(type: "int", nullable: false),
                    turn_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    user_name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_group_storage", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_group_storage_t_machine_model_machine_model_id",
                        column: x => x.machine_model_id,
                        principalTable: "t_machine_model",
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
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    data = table.Column<string>(type: "text", nullable: true),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    remark = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    result = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    test_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    total = table.Column<int>(type: "int", nullable: false)
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
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    data = table.Column<string>(type: "text", nullable: true),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    remark = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    result = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    test_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    total = table.Column<int>(type: "int", nullable: false)
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
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    data = table.Column<string>(type: "text", nullable: true),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    modified_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    remark = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    result = table.Column<string>(type: "varchar(64) CHARACTER SET utf8mb4", maxLength: 64, nullable: true),
                    test_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    total = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_t_group_deliver_machine_model_id",
                table: "t_group_deliver",
                column: "machine_model_id");

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
                name: "IX_t_group_inspection_machine_model_id",
                table: "t_group_inspection",
                column: "machine_model_id");

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
                name: "IX_t_group_storage_machine_model_id",
                table: "t_group_storage",
                column: "machine_model_id");

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
    }
}
