using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Update_AccountMoveLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SalaryPaymentId",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_SalaryPaymentId",
                table: "AccountMoveLines",
                column: "SalaryPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_SalaryPayments_SalaryPaymentId",
                table: "AccountMoveLines",
                column: "SalaryPaymentId",
                principalTable: "SalaryPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_SalaryPayments_SalaryPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_SalaryPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "SalaryPaymentId",
                table: "AccountMoveLines");
        }
    }
}
