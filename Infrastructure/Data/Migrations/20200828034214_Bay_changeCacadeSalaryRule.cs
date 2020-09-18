using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_changeCacadeSalaryRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrSalaryRules_HrPayrollStructures_StructId",
                table: "HrSalaryRules");

            migrationBuilder.AddForeignKey(
                name: "FK_HrSalaryRules_HrPayrollStructures_StructId",
                table: "HrSalaryRules",
                column: "StructId",
                principalTable: "HrPayrollStructures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrSalaryRules_HrPayrollStructures_StructId",
                table: "HrSalaryRules");

            migrationBuilder.AddForeignKey(
                name: "FK_HrSalaryRules_HrPayrollStructures_StructId",
                table: "HrSalaryRules",
                column: "StructId",
                principalTable: "HrPayrollStructures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
