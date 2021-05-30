using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_addProductInSaleOrderPromotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "SaleOrderPromotions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_ProductId",
                table: "SaleOrderPromotions",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderPromotions_Products_ProductId",
                table: "SaleOrderPromotions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderPromotions_Products_ProductId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderPromotions_ProductId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "SaleOrderPromotions");
        }
    }
}
