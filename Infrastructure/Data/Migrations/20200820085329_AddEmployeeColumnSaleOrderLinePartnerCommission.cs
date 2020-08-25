using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddEmployeeColumnSaleOrderLinePartnerCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "SaleOrderLinePartnerCommissions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePartnerCommissions_EmployeeId",
                table: "SaleOrderLinePartnerCommissions",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLinePartnerCommissions_Employees_EmployeeId",
                table: "SaleOrderLinePartnerCommissions",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLinePartnerCommissions_Employees_EmployeeId",
                table: "SaleOrderLinePartnerCommissions");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLinePartnerCommissions_EmployeeId",
                table: "SaleOrderLinePartnerCommissions");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "SaleOrderLinePartnerCommissions");
        }
    }
}
