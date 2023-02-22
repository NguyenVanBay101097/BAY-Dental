using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddEmployeeSaleOrderLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_EmployeeId",
                table: "SaleOrderLines",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_Employees_EmployeeId",
                table: "SaleOrderLines",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_Employees_EmployeeId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_EmployeeId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "SaleOrderLines");
        }
    }
}
