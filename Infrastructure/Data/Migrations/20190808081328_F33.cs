using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F33 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "RoutingLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoutingId",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "DotKhamLineOperations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutingLines_ProductId",
                table: "RoutingLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_RoutingId",
                table: "DotKhamLines",
                column: "RoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_UserId",
                table: "DotKhamLines",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLineOperations_ProductId",
                table: "DotKhamLineOperations",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLineOperations_Products_ProductId",
                table: "DotKhamLineOperations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_Routings_RoutingId",
                table: "DotKhamLines",
                column: "RoutingId",
                principalTable: "Routings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_AspNetUsers_UserId",
                table: "DotKhamLines",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutingLines_Products_ProductId",
                table: "RoutingLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLineOperations_Products_ProductId",
                table: "DotKhamLineOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_Routings_RoutingId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_AspNetUsers_UserId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutingLines_Products_ProductId",
                table: "RoutingLines");

            migrationBuilder.DropIndex(
                name: "IX_RoutingLines_ProductId",
                table: "RoutingLines");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLines_RoutingId",
                table: "DotKhamLines");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLines_UserId",
                table: "DotKhamLines");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLineOperations_ProductId",
                table: "DotKhamLineOperations");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "RoutingLines");

            migrationBuilder.DropColumn(
                name: "RoutingId",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "DotKhamLineOperations");
        }
    }
}
