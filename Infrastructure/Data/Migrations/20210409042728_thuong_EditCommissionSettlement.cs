using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_EditCommissionSettlement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentFixed",
                table: "CommissionProductRules");

            migrationBuilder.AddColumn<Guid>(
                name: "CommissionId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MoveLineId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentAdvisory",
                table: "CommissionProductRules",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentAssistant",
                table: "CommissionProductRules",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentDoctor",
                table: "CommissionProductRules",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_CommissionId",
                table: "CommissionSettlements",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_MoveLineId",
                table: "CommissionSettlements",
                column: "MoveLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_ProductId",
                table: "CommissionSettlements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_SaleOrderId",
                table: "CommissionSettlements",
                column: "SaleOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_Commissions_CommissionId",
                table: "CommissionSettlements",
                column: "CommissionId",
                principalTable: "Commissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_AccountMoveLines_MoveLineId",
                table: "CommissionSettlements",
                column: "MoveLineId",
                principalTable: "AccountMoveLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_Products_ProductId",
                table: "CommissionSettlements",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_SaleOrders_SaleOrderId",
                table: "CommissionSettlements",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Commissions_CommissionId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_AccountMoveLines_MoveLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Products_ProductId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_SaleOrders_SaleOrderId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_CommissionId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_MoveLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_ProductId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_SaleOrderId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "CommissionId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "MoveLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "SaleOrderId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "PercentAdvisory",
                table: "CommissionProductRules");

            migrationBuilder.DropColumn(
                name: "PercentAssistant",
                table: "CommissionProductRules");

            migrationBuilder.DropColumn(
                name: "PercentDoctor",
                table: "CommissionProductRules");

            migrationBuilder.AddColumn<decimal>(
                name: "PercentFixed",
                table: "CommissionProductRules",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
