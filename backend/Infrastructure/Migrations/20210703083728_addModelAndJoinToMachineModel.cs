using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class addModelAndJoinToMachineModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "model_id",
                table: "t_machine_model",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "t_model",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at_utc = table.Column<DateTime>(nullable: false),
                    modified_at_utc = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(maxLength: 128, nullable: true),
                    status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_model", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_machine_model_model_id",
                table: "t_machine_model",
                column: "model_id");
            
            // migrationBuilder.AddForeignKey(
            //     name: "FK_t_machine_model_t_model_model_id",
            //     table: "t_machine_model",
            //     column: "model_id",
            //     principalTable: "t_model",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_t_machine_model_t_model_model_id",
            //     table: "t_machine_model");

            migrationBuilder.DropTable(
                name: "t_model");

            migrationBuilder.DropIndex(
                name: "IX_t_machine_model_model_id",
                table: "t_machine_model");

            migrationBuilder.DropColumn(
                name: "model_id",
                table: "t_machine_model");
        }
    }
}
