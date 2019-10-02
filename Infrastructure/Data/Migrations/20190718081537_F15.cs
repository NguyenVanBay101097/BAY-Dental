using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceLineId",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_InvoiceLineId",
                table: "DotKhamLines",
                column: "InvoiceLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_AccountInvoiceLines_InvoiceLineId",
                table: "DotKhamLines",
                column: "InvoiceLineId",
                principalTable: "AccountInvoiceLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_AccountInvoiceLines_InvoiceLineId",
                table: "DotKhamLines");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLines_InvoiceLineId",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "InvoiceLineId",
                table: "DotKhamLines");
        }
    }
}
