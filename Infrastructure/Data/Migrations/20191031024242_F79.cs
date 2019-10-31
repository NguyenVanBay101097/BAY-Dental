using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F79 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "QtyInvoiced",
                table: "LaboOrderLines",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "LaboOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LaboLineId",
                table: "AccountInvoiceLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_LaboLineId",
                table: "AccountInvoiceLines",
                column: "LaboLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_LaboOrderLines_LaboLineId",
                table: "AccountInvoiceLines",
                column: "LaboLineId",
                principalTable: "LaboOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoiceLines_LaboOrderLines_LaboLineId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountInvoiceLines_LaboLineId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropColumn(
                name: "QtyInvoiced",
                table: "LaboOrderLines");

            migrationBuilder.DropColumn(
                name: "State",
                table: "LaboOrderLines");

            migrationBuilder.DropColumn(
                name: "LaboLineId",
                table: "AccountInvoiceLines");
        }
    }
}
