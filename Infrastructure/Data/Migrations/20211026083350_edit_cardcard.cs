using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class edit_cardcard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CardCardId",
                table: "SaleOrderPromotions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "CardTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_CardCardId",
                table: "SaleOrderPromotions",
                column: "CardCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderPromotions_CardCards_CardCardId",
                table: "SaleOrderPromotions",
                column: "CardCardId",
                principalTable: "CardCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderPromotions_CardCards_CardCardId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderPromotions_CardCardId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropColumn(
                name: "CardCardId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "CardTypes");
        }
    }
}
