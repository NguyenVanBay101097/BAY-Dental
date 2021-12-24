using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddUoMSaleProductionLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleProductionLines_Products_ProductId",
                table: "SaleProductionLines");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductUOMId",
                table: "SaleProductionLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductionLines_ProductUOMId",
                table: "SaleProductionLines",
                column: "ProductUOMId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleProductionLines_Products_ProductId",
                table: "SaleProductionLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleProductionLines_UoMs_ProductUOMId",
                table: "SaleProductionLines",
                column: "ProductUOMId",
                principalTable: "UoMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleProductionLines_Products_ProductId",
                table: "SaleProductionLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleProductionLines_UoMs_ProductUOMId",
                table: "SaleProductionLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleProductionLines_ProductUOMId",
                table: "SaleProductionLines");

            migrationBuilder.DropColumn(
                name: "ProductUOMId",
                table: "SaleProductionLines");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleProductionLines_Products_ProductId",
                table: "SaleProductionLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
