using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddCounselorSaleOrderLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderPayments_AccountMoves_PaymentMoveId",
                table: "SaleOrderPayments");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderPayments_PaymentMoveId",
                table: "SaleOrderPayments");

            migrationBuilder.DropColumn(
                name: "PaymentMoveId",
                table: "SaleOrderPayments");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SaleOrderPayments",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CounselorId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderPaymentId",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_CounselorId",
                table: "SaleOrderLines",
                column: "CounselorId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_SaleOrderPaymentId",
                table: "AccountMoveLines",
                column: "SaleOrderPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_SaleOrderPayments_SaleOrderPaymentId",
                table: "AccountMoveLines",
                column: "SaleOrderPaymentId",
                principalTable: "SaleOrderPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_Employees_CounselorId",
                table: "SaleOrderLines",
                column: "CounselorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_SaleOrderPayments_SaleOrderPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_Employees_CounselorId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_CounselorId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_SaleOrderPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SaleOrderPayments");

            migrationBuilder.DropColumn(
                name: "CounselorId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "SaleOrderPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMoveId",
                table: "SaleOrderPayments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPayments_PaymentMoveId",
                table: "SaleOrderPayments",
                column: "PaymentMoveId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderPayments_AccountMoves_PaymentMoveId",
                table: "SaleOrderPayments",
                column: "PaymentMoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
