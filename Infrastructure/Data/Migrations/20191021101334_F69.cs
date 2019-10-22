using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F69 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PartnerCategId",
                table: "ProductPricelists",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerCategId",
                table: "ProductPricelistItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_PartnerCategId",
                table: "ProductPricelists",
                column: "PartnerCategId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_PartnerCategId",
                table: "ProductPricelistItems",
                column: "PartnerCategId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPricelistItems_PartnerCategories_PartnerCategId",
                table: "ProductPricelistItems",
                column: "PartnerCategId",
                principalTable: "PartnerCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPricelists_PartnerCategories_PartnerCategId",
                table: "ProductPricelists",
                column: "PartnerCategId",
                principalTable: "PartnerCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPricelistItems_PartnerCategories_PartnerCategId",
                table: "ProductPricelistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPricelists_PartnerCategories_PartnerCategId",
                table: "ProductPricelists");

            migrationBuilder.DropIndex(
                name: "IX_ProductPricelists_PartnerCategId",
                table: "ProductPricelists");

            migrationBuilder.DropIndex(
                name: "IX_ProductPricelistItems_PartnerCategId",
                table: "ProductPricelistItems");

            migrationBuilder.DropColumn(
                name: "PartnerCategId",
                table: "ProductPricelists");

            migrationBuilder.DropColumn(
                name: "PartnerCategId",
                table: "ProductPricelistItems");
        }
    }
}
