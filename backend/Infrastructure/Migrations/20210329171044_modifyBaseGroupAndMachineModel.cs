using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class modifyBaseGroupAndMachineModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_group_t_machine_machine_id",
                table: "t_group");

            migrationBuilder.DropForeignKey(
                name: "FK_t_group_deliver_t_machine_machine_id",
                table: "t_group_deliver");

            migrationBuilder.DropForeignKey(
                name: "FK_t_group_inspection_t_machine_machine_id",
                table: "t_group_inspection");

            migrationBuilder.DropForeignKey(
                name: "FK_t_group_storage_t_machine_machine_id",
                table: "t_group_storage");

            migrationBuilder.DropIndex(
                name: "IX_t_group_storage_machine_id",
                table: "t_group_storage");

            migrationBuilder.DropIndex(
                name: "IX_t_group_inspection_machine_id",
                table: "t_group_inspection");

            migrationBuilder.DropIndex(
                name: "IX_t_group_deliver_machine_id",
                table: "t_group_deliver");

            migrationBuilder.DropIndex(
                name: "IX_t_group_machine_id",
                table: "t_group");

            migrationBuilder.DropColumn(
                name: "machine_id",
                table: "t_group_storage");

            migrationBuilder.DropColumn(
                name: "machine_id",
                table: "t_group_inspection");

            migrationBuilder.DropColumn(
                name: "machine_id",
                table: "t_group_deliver");

            migrationBuilder.DropColumn(
                name: "machine_id",
                table: "t_group");

            migrationBuilder.AddColumn<int>(
                name: "work_shop_id",
                table: "t_machine_model",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "machine_model_id",
                table: "t_group_storage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "machine_model_id",
                table: "t_group_inspection",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "machine_model_id",
                table: "t_group_deliver",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "machine_model_id",
                table: "t_group",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_t_machine_model_work_shop_id",
                table: "t_machine_model",
                column: "work_shop_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_storage_machine_model_id",
                table: "t_group_storage",
                column: "machine_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_inspection_machine_model_id",
                table: "t_group_inspection",
                column: "machine_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_deliver_machine_model_id",
                table: "t_group_deliver",
                column: "machine_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_machine_model_id",
                table: "t_group",
                column: "machine_model_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_t_machine_model_machine_model_id",
                table: "t_group",
                column: "machine_model_id",
                principalTable: "t_machine_model",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_deliver_t_machine_model_machine_model_id",
                table: "t_group_deliver",
                column: "machine_model_id",
                principalTable: "t_machine_model",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_inspection_t_machine_model_machine_model_id",
                table: "t_group_inspection",
                column: "machine_model_id",
                principalTable: "t_machine_model",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_storage_t_machine_model_machine_model_id",
                table: "t_group_storage",
                column: "machine_model_id",
                principalTable: "t_machine_model",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_machine_model_t_work_shop_work_shop_id",
                table: "t_machine_model",
                column: "work_shop_id",
                principalTable: "t_work_shop",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_group_t_machine_model_machine_model_id",
                table: "t_group");

            migrationBuilder.DropForeignKey(
                name: "FK_t_group_deliver_t_machine_model_machine_model_id",
                table: "t_group_deliver");

            migrationBuilder.DropForeignKey(
                name: "FK_t_group_inspection_t_machine_model_machine_model_id",
                table: "t_group_inspection");

            migrationBuilder.DropForeignKey(
                name: "FK_t_group_storage_t_machine_model_machine_model_id",
                table: "t_group_storage");

            migrationBuilder.DropForeignKey(
                name: "FK_t_machine_model_t_work_shop_work_shop_id",
                table: "t_machine_model");

            migrationBuilder.DropIndex(
                name: "IX_t_machine_model_work_shop_id",
                table: "t_machine_model");

            migrationBuilder.DropIndex(
                name: "IX_t_group_storage_machine_model_id",
                table: "t_group_storage");

            migrationBuilder.DropIndex(
                name: "IX_t_group_inspection_machine_model_id",
                table: "t_group_inspection");

            migrationBuilder.DropIndex(
                name: "IX_t_group_deliver_machine_model_id",
                table: "t_group_deliver");

            migrationBuilder.DropIndex(
                name: "IX_t_group_machine_model_id",
                table: "t_group");

            migrationBuilder.DropColumn(
                name: "work_shop_id",
                table: "t_machine_model");

            migrationBuilder.DropColumn(
                name: "machine_model_id",
                table: "t_group_storage");

            migrationBuilder.DropColumn(
                name: "machine_model_id",
                table: "t_group_inspection");

            migrationBuilder.DropColumn(
                name: "machine_model_id",
                table: "t_group_deliver");

            migrationBuilder.DropColumn(
                name: "machine_model_id",
                table: "t_group");

            migrationBuilder.AddColumn<int>(
                name: "machine_id",
                table: "t_group_storage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "machine_id",
                table: "t_group_inspection",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "machine_id",
                table: "t_group_deliver",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "machine_id",
                table: "t_group",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_t_group_storage_machine_id",
                table: "t_group_storage",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_inspection_machine_id",
                table: "t_group_inspection",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_deliver_machine_id",
                table: "t_group_deliver",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_group_machine_id",
                table: "t_group",
                column: "machine_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_t_machine_machine_id",
                table: "t_group",
                column: "machine_id",
                principalTable: "t_machine",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_deliver_t_machine_machine_id",
                table: "t_group_deliver",
                column: "machine_id",
                principalTable: "t_machine",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_inspection_t_machine_machine_id",
                table: "t_group_inspection",
                column: "machine_id",
                principalTable: "t_machine",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_storage_t_machine_machine_id",
                table: "t_group_storage",
                column: "machine_id",
                principalTable: "t_machine",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
