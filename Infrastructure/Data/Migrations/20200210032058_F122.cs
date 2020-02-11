using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LaboPrice",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderId",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_SaleOrderId",
                table: "LaboOrders",
                column: "SaleOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_SaleOrders_SaleOrderId",
                table: "LaboOrders",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_SaleOrders_SaleOrderId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_SaleOrderId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "LaboPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SaleOrderId",
                table: "LaboOrders");
        }
    }
}
