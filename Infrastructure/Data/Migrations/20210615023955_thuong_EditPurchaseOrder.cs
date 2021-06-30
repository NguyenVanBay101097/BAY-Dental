using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_EditPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPayment",
                table: "PurchaseOrders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountResidual",
                table: "PurchaseOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "JournalId",
                table: "PurchaseOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_JournalId",
                table: "PurchaseOrders",
                column: "JournalId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_AccountJournals_JournalId",
                table: "PurchaseOrders",
                column: "JournalId",
                principalTable: "AccountJournals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_AccountJournals_JournalId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_JournalId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "AmountPayment",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "AmountResidual",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "JournalId",
                table: "PurchaseOrders");
        }
    }
}
