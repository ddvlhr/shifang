using Microsoft.EntityFrameworkCore.Migrations;

namespace FuYang.Infrastructure.Migrations
{
    public partial class modifySystemSettingColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "t_system_setting",
                newName: "weight");

            migrationBuilder.RenameColumn(
                name: "Resistance",
                table: "t_system_setting",
                newName: "resistance");

            migrationBuilder.RenameColumn(
                name: "Oval",
                table: "t_system_setting",
                newName: "oval");

            migrationBuilder.RenameColumn(
                name: "Length",
                table: "t_system_setting",
                newName: "length");

            migrationBuilder.RenameColumn(
                name: "Hardness",
                table: "t_system_setting",
                newName: "hardness");

            migrationBuilder.RenameColumn(
                name: "Circle",
                table: "t_system_setting",
                newName: "circle");

            migrationBuilder.RenameColumn(
                name: "ChemicalTypeId",
                table: "t_system_setting",
                newName: "chemical_type_id");

            migrationBuilder.RenameColumn(
                name: "CanSeeOtherData",
                table: "t_system_setting",
                newName: "can_see_other_data");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "t_system_setting",
                newName: "admin_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "weight",
                table: "t_system_setting",
                newName: "Weight");

            migrationBuilder.RenameColumn(
                name: "resistance",
                table: "t_system_setting",
                newName: "Resistance");

            migrationBuilder.RenameColumn(
                name: "oval",
                table: "t_system_setting",
                newName: "Oval");

            migrationBuilder.RenameColumn(
                name: "length",
                table: "t_system_setting",
                newName: "Length");

            migrationBuilder.RenameColumn(
                name: "hardness",
                table: "t_system_setting",
                newName: "Hardness");

            migrationBuilder.RenameColumn(
                name: "circle",
                table: "t_system_setting",
                newName: "Circle");

            migrationBuilder.RenameColumn(
                name: "chemical_type_id",
                table: "t_system_setting",
                newName: "ChemicalTypeId");

            migrationBuilder.RenameColumn(
                name: "can_see_other_data",
                table: "t_system_setting",
                newName: "CanSeeOtherData");

            migrationBuilder.RenameColumn(
                name: "admin_id",
                table: "t_system_setting",
                newName: "AdminId");
        }
    }
}
