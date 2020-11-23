using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_EditV2_TableSalaryPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "MoveId",
                table: "SalaryPayments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalaryPayments_MoveId",
                table: "SalaryPayments",
                column: "MoveId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayments_AccountMoves_MoveId",
                table: "SalaryPayments",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayments_AccountMoves_MoveId",
                table: "SalaryPayments");

            migrationBuilder.DropIndex(
                name: "IX_SalaryPayments_MoveId",
                table: "SalaryPayments");

            migrationBuilder.DropColumn(
                name: "MoveId",
                table: "SalaryPayments");

            migrationBuilder.AddColumn<Guid>(
                name: "SalaryPaymentId",
                table: "AccountMoveLines",
                type: "uniqueidentifier",
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
    }
}
