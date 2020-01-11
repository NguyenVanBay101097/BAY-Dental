using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F114 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RewardProductId",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RewardProductQuantity",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponPrograms_RewardProductId",
                table: "SaleCouponPrograms",
                column: "RewardProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleCouponPrograms_Products_RewardProductId",
                table: "SaleCouponPrograms",
                column: "RewardProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleCouponPrograms_Products_RewardProductId",
                table: "SaleCouponPrograms");

            migrationBuilder.DropIndex(
                name: "IX_SaleCouponPrograms_RewardProductId",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "RewardProductId",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "RewardProductQuantity",
                table: "SaleCouponPrograms");
        }
    }
}
