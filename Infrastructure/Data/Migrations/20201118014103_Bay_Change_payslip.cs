using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_Change_payslip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "HrPayslipRuns");

            migrationBuilder.AddColumn<decimal>(
                name: "WorkedDay",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "HrPayslipRuns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkedDay",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "HrPayslipRuns");

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "HrPayslipRuns",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
