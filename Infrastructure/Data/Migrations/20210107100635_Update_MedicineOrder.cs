using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Update_MedicineOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrders_Employees_EmployeeId",
                table: "MedicineOrders");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "MedicineOrders",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineOrders_Employees_EmployeeId",
                table: "MedicineOrders",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrders_AccountPayments_AccountPaymentId",
                table: "MedicineOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrders_Employees_EmployeeId",
                table: "MedicineOrders");

            migrationBuilder.DropIndex(
                name: "IX_MedicineOrders_AccountPaymentId",
                table: "MedicineOrders");

            migrationBuilder.DropColumn(
                name: "AccountPaymentId",
                table: "MedicineOrders");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "MedicineOrders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineOrders_Employees_EmployeeId",
                table: "MedicineOrders",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
