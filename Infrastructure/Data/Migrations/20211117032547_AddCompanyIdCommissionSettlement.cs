using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddCompanyIdCommissionSettlement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_CompanyId",
                table: "CommissionSettlements",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_Companies_CompanyId",
                table: "CommissionSettlements",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Companies_CompanyId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_CompanyId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CommissionSettlements");
        }
    }
}
