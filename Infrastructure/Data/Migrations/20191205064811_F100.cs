using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PromotionId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountLineProductId",
                table: "PromotionRules",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountLineProductId",
                table: "PromotionRuleProductRels",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountLineProductId",
                table: "PromotionRuleProductCategoryRels",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_PromotionId",
                table: "SaleOrderLines",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRules_DiscountLineProductId",
                table: "PromotionRules",
                column: "DiscountLineProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleProductRels_DiscountLineProductId",
                table: "PromotionRuleProductRels",
                column: "DiscountLineProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleProductCategoryRels_DiscountLineProductId",
                table: "PromotionRuleProductCategoryRels",
                column: "DiscountLineProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionRuleProductCategoryRels_Products_DiscountLineProductId",
                table: "PromotionRuleProductCategoryRels",
                column: "DiscountLineProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionRuleProductRels_Products_DiscountLineProductId",
                table: "PromotionRuleProductRels",
                column: "DiscountLineProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionRules_Products_DiscountLineProductId",
                table: "PromotionRules",
                column: "DiscountLineProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_PromotionPrograms_PromotionId",
                table: "SaleOrderLines",
                column: "PromotionId",
                principalTable: "PromotionPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionRuleProductCategoryRels_Products_DiscountLineProductId",
                table: "PromotionRuleProductCategoryRels");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionRuleProductRels_Products_DiscountLineProductId",
                table: "PromotionRuleProductRels");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionRules_Products_DiscountLineProductId",
                table: "PromotionRules");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_PromotionPrograms_PromotionId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_PromotionId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_PromotionRules_DiscountLineProductId",
                table: "PromotionRules");

            migrationBuilder.DropIndex(
                name: "IX_PromotionRuleProductRels_DiscountLineProductId",
                table: "PromotionRuleProductRels");

            migrationBuilder.DropIndex(
                name: "IX_PromotionRuleProductCategoryRels_DiscountLineProductId",
                table: "PromotionRuleProductCategoryRels");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "DiscountLineProductId",
                table: "PromotionRules");

            migrationBuilder.DropColumn(
                name: "DiscountLineProductId",
                table: "PromotionRuleProductRels");

            migrationBuilder.DropColumn(
                name: "DiscountLineProductId",
                table: "PromotionRuleProductCategoryRels");
        }
    }
}
