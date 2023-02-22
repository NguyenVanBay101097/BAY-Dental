using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdatePurchaseInvoiceColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateMaturity",
                table: "AccountMoveLines",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseLineId",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_PurchaseLineId",
                table: "AccountMoveLines",
                column: "PurchaseLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_PurchaseOrderLines_PurchaseLineId",
                table: "AccountMoveLines",
                column: "PurchaseLineId",
                principalTable: "PurchaseOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_PurchaseOrderLines_PurchaseLineId",
                table: "AccountMoveLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_PurchaseLineId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "PurchaseLineId",
                table: "AccountMoveLines");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateMaturity",
                table: "AccountMoveLines",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
