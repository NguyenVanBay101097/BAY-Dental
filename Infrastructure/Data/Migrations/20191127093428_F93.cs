using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F93 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "CardTypes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "PricelistId",
                table: "CardTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardTypes_PricelistId",
                table: "CardTypes",
                column: "PricelistId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardTypes_ProductPricelists_PricelistId",
                table: "CardTypes",
                column: "PricelistId",
                principalTable: "ProductPricelists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardTypes_ProductPricelists_PricelistId",
                table: "CardTypes");

            migrationBuilder.DropIndex(
                name: "IX_CardTypes_PricelistId",
                table: "CardTypes");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "CardTypes");

            migrationBuilder.DropColumn(
                name: "PricelistId",
                table: "CardTypes");
        }
    }
}
