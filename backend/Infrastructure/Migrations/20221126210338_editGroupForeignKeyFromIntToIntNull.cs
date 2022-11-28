using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuYang.Infrastructure.Migrations
{
    public partial class editGroupForeignKeyFromIntToIntNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<int>(
            //     name: "team_id",
            //     table: "t_group_record",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.AddColumn<int>(
            //     name: "count",
            //     table: "t_group",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.AddColumn<int>(
            //     name: "team_id",
            //     table: "t_group",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.CreateIndex(
            //     name: "IX_t_group_record_team_id",
            //     table: "t_group_record",
            //     column: "team_id");

            // migrationBuilder.CreateIndex(
            //     name: "IX_t_group_team_id",
            //     table: "t_group",
            //     column: "team_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_t_team_team_id",
                table: "t_group",
                column: "team_id",
                principalTable: "t_team",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_group_record_t_team_team_id",
                table: "t_group_record",
                column: "team_id",
                principalTable: "t_team",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_group_t_team_team_id",
                table: "t_group");

            migrationBuilder.DropForeignKey(
                name: "FK_t_group_record_t_team_team_id",
                table: "t_group_record");

            // migrationBuilder.DropIndex(
            //     name: "IX_t_group_record_team_id",
            //     table: "t_group_record");

            // migrationBuilder.DropIndex(
            //     name: "IX_t_group_team_id",
            //     table: "t_group");

            // migrationBuilder.DropColumn(
            //     name: "team_id",
            //     table: "t_group_record");

            // migrationBuilder.DropColumn(
            //     name: "count",
            //     table: "t_group");

            // migrationBuilder.DropColumn(
            //     name: "team_id",
            //     table: "t_group");
        }
    }
}
