using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F96 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PricelistId",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_PricelistId",
                table: "SaleOrders",
                column: "PricelistId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_ProductPricelists_PricelistId",
                table: "SaleOrders",
                column: "PricelistId",
                principalTable: "ProductPricelists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_ProductPricelists_PricelistId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_PricelistId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "PricelistId",
                table: "SaleOrders");
        }
    }
}
