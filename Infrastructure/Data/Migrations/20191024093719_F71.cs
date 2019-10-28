using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F71 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductUOMId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_ProductUOMId",
                table: "SaleOrderLines",
                column: "ProductUOMId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_UoMs_ProductUOMId",
                table: "SaleOrderLines",
                column: "ProductUOMId",
                principalTable: "UoMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_UoMs_ProductUOMId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_ProductUOMId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "ProductUOMId",
                table: "SaleOrderLines");
        }
    }
}
