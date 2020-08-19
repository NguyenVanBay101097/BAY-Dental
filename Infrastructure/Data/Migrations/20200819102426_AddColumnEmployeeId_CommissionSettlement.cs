using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddColumnEmployeeId_CommissionSettlement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_EmployeeId",
                table: "CommissionSettlements",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_Employees_EmployeeId",
                table: "CommissionSettlements",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Employees_EmployeeId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_EmployeeId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "CommissionSettlements");
        }
    }
}
