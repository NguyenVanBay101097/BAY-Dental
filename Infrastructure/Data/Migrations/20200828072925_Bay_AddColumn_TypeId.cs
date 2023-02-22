using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_AddColumn_TypeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StructureTypeId",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_StructureTypeId",
                table: "HrPayslips",
                column: "StructureTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayslips_HrPayrollStructureTypes_StructureTypeId",
                table: "HrPayslips",
                column: "StructureTypeId",
                principalTable: "HrPayrollStructureTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayslips_HrPayrollStructureTypes_StructureTypeId",
                table: "HrPayslips");

            migrationBuilder.DropIndex(
                name: "IX_HrPayslips_StructureTypeId",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "StructureTypeId",
                table: "HrPayslips");
        }
    }
}
