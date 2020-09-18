using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddCompanyColumnPayrollStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "HrPayrollStructureTypes",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "HrPayrollStructures",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructureTypes_CompanyId",
                table: "HrPayrollStructureTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructures_CompanyId",
                table: "HrPayrollStructures",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayrollStructures_Companies_CompanyId",
                table: "HrPayrollStructures",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayrollStructureTypes_Companies_CompanyId",
                table: "HrPayrollStructureTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayrollStructures_Companies_CompanyId",
                table: "HrPayrollStructures");

            migrationBuilder.DropForeignKey(
                name: "FK_HrPayrollStructureTypes_Companies_CompanyId",
                table: "HrPayrollStructureTypes");

            migrationBuilder.DropIndex(
                name: "IX_HrPayrollStructureTypes_CompanyId",
                table: "HrPayrollStructureTypes");

            migrationBuilder.DropIndex(
                name: "IX_HrPayrollStructures_CompanyId",
                table: "HrPayrollStructures");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "HrPayrollStructureTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "HrPayrollStructures");
        }
    }
}
