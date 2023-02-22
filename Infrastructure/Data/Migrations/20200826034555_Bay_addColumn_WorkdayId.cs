using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_addColumn_WorkdayId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkEntryTypeId",
                table: "HrPayslipWorkedDays",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipWorkedDays_WorkEntryTypeId",
                table: "HrPayslipWorkedDays",
                column: "WorkEntryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayslipWorkedDays_WorkEntryTypes_WorkEntryTypeId",
                table: "HrPayslipWorkedDays",
                column: "WorkEntryTypeId",
                principalTable: "WorkEntryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayslipWorkedDays_WorkEntryTypes_WorkEntryTypeId",
                table: "HrPayslipWorkedDays");

            migrationBuilder.DropIndex(
                name: "IX_HrPayslipWorkedDays_WorkEntryTypeId",
                table: "HrPayslipWorkedDays");

            migrationBuilder.DropColumn(
                name: "WorkEntryTypeId",
                table: "HrPayslipWorkedDays");
        }
    }
}
