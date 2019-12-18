using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F107 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuoteId",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_OrderId",
                table: "SaleOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_QuoteId",
                table: "SaleOrders",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_SaleOrders_OrderId",
                table: "SaleOrders",
                column: "OrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_SaleOrders_QuoteId",
                table: "SaleOrders",
                column: "QuoteId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_SaleOrders_OrderId",
                table: "SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_SaleOrders_QuoteId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_OrderId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_QuoteId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "QuoteId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SaleOrders");
        }
    }
}
