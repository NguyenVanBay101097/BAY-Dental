using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F35 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DotKhamLineOperations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoutingLines",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoutingLines",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceLineId",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DotKhamLineOperations",
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
    }
}
