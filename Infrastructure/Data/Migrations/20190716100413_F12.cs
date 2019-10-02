using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ToothCategoryId",
                table: "AccountInvoiceLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_ToothCategoryId",
                table: "AccountInvoiceLines",
                column: "ToothCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_ToothCategories_ToothCategoryId",
                table: "AccountInvoiceLines",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoiceLines_ToothCategories_ToothCategoryId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountInvoiceLines_ToothCategoryId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropColumn(
                name: "ToothCategoryId",
                table: "AccountInvoiceLines");
        }
    }
}
