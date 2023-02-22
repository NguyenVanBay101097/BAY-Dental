using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddStockPicking_PurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PickingId",
                table: "PurchaseOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PickingId",
                table: "PurchaseOrders",
                column: "PickingId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_StockPickings_PickingId",
                table: "PurchaseOrders",
                column: "PickingId",
                principalTable: "StockPickings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_StockPickings_PickingId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_PickingId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "PickingId",
                table: "PurchaseOrders");
        }
    }
}
