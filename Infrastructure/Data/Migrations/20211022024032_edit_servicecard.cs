using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class edit_servicecard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductPricelistId",
                table: "ServiceCardTypes",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceCardCardId",
                table: "SaleOrderPromotions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardTypes_ProductPricelistId",
                table: "ServiceCardTypes",
                column: "ProductPricelistId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_ServiceCardCardId",
                table: "SaleOrderPromotions",
                column: "ServiceCardCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderPromotions_ServiceCardCards_ServiceCardCardId",
                table: "SaleOrderPromotions",
                column: "ServiceCardCardId",
                principalTable: "ServiceCardCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCardTypes_ProductPricelists_ProductPricelistId",
                table: "ServiceCardTypes",
                column: "ProductPricelistId",
                principalTable: "ProductPricelists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderPromotions_ServiceCardCards_ServiceCardCardId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCardTypes_ProductPricelists_ProductPricelistId",
                table: "ServiceCardTypes");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCardTypes_ProductPricelistId",
                table: "ServiceCardTypes");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderPromotions_ServiceCardCardId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropColumn(
                name: "ProductPricelistId",
                table: "ServiceCardTypes");

            migrationBuilder.DropColumn(
                name: "ServiceCardCardId",
                table: "SaleOrderPromotions");
        }
    }
}
