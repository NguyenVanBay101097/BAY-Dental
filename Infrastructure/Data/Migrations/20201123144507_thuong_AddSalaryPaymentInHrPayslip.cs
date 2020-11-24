using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddSalaryPaymentInHrPayslip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SalaryPaymentId",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_SalaryPaymentId",
                table: "HrPayslips",
                column: "SalaryPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayslips_SalaryPayments_SalaryPaymentId",
                table: "HrPayslips",
                column: "SalaryPaymentId",
                principalTable: "SalaryPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayslips_SalaryPayments_SalaryPaymentId",
                table: "HrPayslips");

            migrationBuilder.DropIndex(
                name: "IX_HrPayslips_SalaryPaymentId",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "SalaryPaymentId",
                table: "HrPayslips");
        }
    }
}
