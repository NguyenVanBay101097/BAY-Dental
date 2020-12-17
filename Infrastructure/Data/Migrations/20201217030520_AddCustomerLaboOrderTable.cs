using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddCustomerLaboOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_CustomerId",
                table: "LaboOrders",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_Partners_CustomerId",
                table: "LaboOrders",
                column: "CustomerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_Partners_CustomerId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_CustomerId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "LaboOrders");
        }
    }
}
