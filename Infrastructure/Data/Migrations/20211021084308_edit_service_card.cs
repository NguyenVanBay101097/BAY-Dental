using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class edit_service_card : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductPricelistId",
                table: "ServiceCardTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardTypes_ProductPricelistId",
                table: "ServiceCardTypes",
                column: "ProductPricelistId");

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
                name: "FK_ServiceCardTypes_ProductPricelists_ProductPricelistId",
                table: "ServiceCardTypes");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCardTypes_ProductPricelistId",
                table: "ServiceCardTypes");

            migrationBuilder.DropColumn(
                name: "ProductPricelistId",
                table: "ServiceCardTypes");
        }
    }
}
