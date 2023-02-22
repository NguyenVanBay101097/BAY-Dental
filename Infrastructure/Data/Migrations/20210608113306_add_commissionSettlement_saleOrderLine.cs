using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class add_commissionSettlement_saleOrderLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderLineId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_SaleOrderLineId",
                table: "CommissionSettlements",
                column: "SaleOrderLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_SaleOrderLines_SaleOrderLineId",
                table: "CommissionSettlements",
                column: "SaleOrderLineId",
                principalTable: "SaleOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_SaleOrderLines_SaleOrderLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_SaleOrderLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "SaleOrderLineId",
                table: "CommissionSettlements");
        }
    }
}
