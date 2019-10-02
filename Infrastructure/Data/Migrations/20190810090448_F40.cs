using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F40 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOrder",
                table: "AccountInvoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "AccountInvoices",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToothId",
                table: "AccountInvoiceLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_ToothId",
                table: "AccountInvoiceLines",
                column: "ToothId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_Teeth_ToothId",
                table: "AccountInvoiceLines",
                column: "ToothId",
                principalTable: "Teeth",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoiceLines_Teeth_ToothId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountInvoiceLines_ToothId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropColumn(
                name: "DateOrder",
                table: "AccountInvoices");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "AccountInvoices");

            migrationBuilder.DropColumn(
                name: "ToothId",
                table: "AccountInvoiceLines");
        }
    }
}
