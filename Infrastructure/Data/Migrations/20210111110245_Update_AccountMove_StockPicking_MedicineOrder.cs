using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Update_AccountMove_StockPicking_MedicineOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MoveId",
                table: "MedicineOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StockPickingIncomingId",
                table: "MedicineOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StockPickingOutgoingId",
                table: "MedicineOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "MedicineOrderLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_MoveId",
                table: "MedicineOrders",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_StockPickingIncomingId",
                table: "MedicineOrders",
                column: "StockPickingIncomingId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_StockPickingOutgoingId",
                table: "MedicineOrders",
                column: "StockPickingOutgoingId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderLines_ProductId",
                table: "MedicineOrderLines",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineOrderLines_Products_ProductId",
                table: "MedicineOrderLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineOrders_AccountMoves_MoveId",
                table: "MedicineOrders",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineOrders_StockPickings_StockPickingIncomingId",
                table: "MedicineOrders",
                column: "StockPickingIncomingId",
                principalTable: "StockPickings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineOrders_StockPickings_StockPickingOutgoingId",
                table: "MedicineOrders",
                column: "StockPickingOutgoingId",
                principalTable: "StockPickings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrderLines_Products_ProductId",
                table: "MedicineOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrders_AccountMoves_MoveId",
                table: "MedicineOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrders_StockPickings_StockPickingIncomingId",
                table: "MedicineOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrders_StockPickings_StockPickingOutgoingId",
                table: "MedicineOrders");

            migrationBuilder.DropIndex(
                name: "IX_MedicineOrders_MoveId",
                table: "MedicineOrders");

            migrationBuilder.DropIndex(
                name: "IX_MedicineOrders_StockPickingIncomingId",
                table: "MedicineOrders");

            migrationBuilder.DropIndex(
                name: "IX_MedicineOrders_StockPickingOutgoingId",
                table: "MedicineOrders");

            migrationBuilder.DropIndex(
                name: "IX_MedicineOrderLines_ProductId",
                table: "MedicineOrderLines");

            migrationBuilder.DropColumn(
                name: "MoveId",
                table: "MedicineOrders");

            migrationBuilder.DropColumn(
                name: "StockPickingIncomingId",
                table: "MedicineOrders");

            migrationBuilder.DropColumn(
                name: "StockPickingOutgoingId",
                table: "MedicineOrders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "MedicineOrderLines");
        }
    }
}
