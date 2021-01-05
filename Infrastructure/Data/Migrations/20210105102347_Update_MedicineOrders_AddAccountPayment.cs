using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Update_MedicineOrders_AddAccountPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountPaymentId",
                table: "MedicineOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_AccountPaymentId",
                table: "MedicineOrders",
                column: "AccountPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineOrders_AccountPayments_AccountPaymentId",
                table: "MedicineOrders",
                column: "AccountPaymentId",
                principalTable: "AccountPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrders_AccountPayments_AccountPaymentId",
                table: "MedicineOrders");

            migrationBuilder.DropIndex(
                name: "IX_MedicineOrders_AccountPaymentId",
                table: "MedicineOrders");

            migrationBuilder.DropColumn(
                name: "AccountPaymentId",
                table: "MedicineOrders");
        }
    }
}
