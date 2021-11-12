using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddHistoryLineCommissionSettlement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HistoryLineId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_HistoryLineId",
                table: "CommissionSettlements",
                column: "HistoryLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_SaleOrderPaymentHistoryLines_HistoryLineId",
                table: "CommissionSettlements",
                column: "HistoryLineId",
                principalTable: "SaleOrderPaymentHistoryLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_SaleOrderPaymentHistoryLines_HistoryLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_HistoryLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "HistoryLineId",
                table: "CommissionSettlements");
        }
    }
}
