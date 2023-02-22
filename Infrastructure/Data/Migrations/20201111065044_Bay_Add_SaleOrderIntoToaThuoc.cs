using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_Add_SaleOrderIntoToaThuoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderID",
                table: "ToaThuocs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_SaleOrderID",
                table: "ToaThuocs",
                column: "SaleOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ToaThuocs_SaleOrders_SaleOrderID",
                table: "ToaThuocs",
                column: "SaleOrderID",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToaThuocs_SaleOrders_SaleOrderID",
                table: "ToaThuocs");

            migrationBuilder.DropIndex(
                name: "IX_ToaThuocs_SaleOrderID",
                table: "ToaThuocs");

            migrationBuilder.DropColumn(
                name: "SaleOrderID",
                table: "ToaThuocs");
        }
    }
}
