using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F73 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamSteps_AccountInvoices_InvoicesId",
                table: "DotKhamSteps");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoicesId",
                table: "DotKhamSteps",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "SaleLineId",
                table: "DotKhamSteps",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderId",
                table: "DotKhamSteps",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoiceId",
                table: "DotKhams",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderId",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_SaleLineId",
                table: "DotKhamSteps",
                column: "SaleLineId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_SaleOrderId",
                table: "DotKhamSteps",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_SaleOrderId",
                table: "DotKhams",
                column: "SaleOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_SaleOrders_SaleOrderId",
                table: "DotKhams",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_AccountInvoices_InvoicesId",
                table: "DotKhamSteps",
                column: "InvoicesId",
                principalTable: "AccountInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_SaleOrderLines_SaleLineId",
                table: "DotKhamSteps",
                column: "SaleLineId",
                principalTable: "SaleOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_SaleOrders_SaleOrderId",
                table: "DotKhamSteps",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_SaleOrders_SaleOrderId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamSteps_AccountInvoices_InvoicesId",
                table: "DotKhamSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamSteps_SaleOrderLines_SaleLineId",
                table: "DotKhamSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamSteps_SaleOrders_SaleOrderId",
                table: "DotKhamSteps");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamSteps_SaleLineId",
                table: "DotKhamSteps");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamSteps_SaleOrderId",
                table: "DotKhamSteps");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_SaleOrderId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "SaleLineId",
                table: "DotKhamSteps");

            migrationBuilder.DropColumn(
                name: "SaleOrderId",
                table: "DotKhamSteps");

            migrationBuilder.DropColumn(
                name: "SaleOrderId",
                table: "DotKhams");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoicesId",
                table: "DotKhamSteps",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoiceId",
                table: "DotKhams",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_AccountInvoices_InvoicesId",
                table: "DotKhamSteps",
                column: "InvoicesId",
                principalTable: "AccountInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
