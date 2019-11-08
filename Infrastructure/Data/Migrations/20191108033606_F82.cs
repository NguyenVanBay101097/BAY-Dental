using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F82 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "StockPickings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "StockMoves",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductQty",
                table: "StockMoves",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductUOMId",
                table: "StockMoves",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseLineId",
                table: "StockMoves",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_ProductUOMId",
                table: "StockMoves",
                column: "ProductUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_PurchaseLineId",
                table: "StockMoves",
                column: "PurchaseLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_UoMs_ProductUOMId",
                table: "StockMoves",
                column: "ProductUOMId",
                principalTable: "UoMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_PurchaseOrderLines_PurchaseLineId",
                table: "StockMoves",
                column: "PurchaseLineId",
                principalTable: "PurchaseOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMoves_UoMs_ProductUOMId",
                table: "StockMoves");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMoves_PurchaseOrderLines_PurchaseLineId",
                table: "StockMoves");

            migrationBuilder.DropIndex(
                name: "IX_StockMoves_ProductUOMId",
                table: "StockMoves");

            migrationBuilder.DropIndex(
                name: "IX_StockMoves_PurchaseLineId",
                table: "StockMoves");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "StockPickings");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "StockMoves");

            migrationBuilder.DropColumn(
                name: "ProductQty",
                table: "StockMoves");

            migrationBuilder.DropColumn(
                name: "ProductUOMId",
                table: "StockMoves");

            migrationBuilder.DropColumn(
                name: "PurchaseLineId",
                table: "StockMoves");
        }
    }
}
