using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AccountMoveInvoiceUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceReduce",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountResidual",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountResidualSigned",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTax",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTaxSigned",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTotal",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTotalSigned",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountUntaxed",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountUntaxedSigned",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceDate",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceOrigin",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoicePaymentRef",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoicePaymentState",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceUserId",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AccountMoves",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountInternalType",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ExcludeFromInvoiceTab",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoveName",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentState",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceSubtotal",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceTotal",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceUnit",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SaleOrderLineInvoice2Rels",
                columns: table => new
                {
                    OrderLineId = table.Column<Guid>(nullable: false),
                    InvoiceLineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLineInvoice2Rels", x => new { x.OrderLineId, x.InvoiceLineId });
                    table.ForeignKey(
                        name: "FK_SaleOrderLineInvoice2Rels_AccountMoveLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "AccountMoveLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLineInvoice2Rels_SaleOrderLines_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoves_InvoiceUserId",
                table: "AccountMoves",
                column: "InvoiceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineInvoice2Rels_InvoiceLineId",
                table: "SaleOrderLineInvoice2Rels",
                column: "InvoiceLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoves_AspNetUsers_InvoiceUserId",
                table: "AccountMoves",
                column: "InvoiceUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoves_AspNetUsers_InvoiceUserId",
                table: "AccountMoves");

            migrationBuilder.DropTable(
                name: "SaleOrderLineInvoice2Rels");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoves_InvoiceUserId",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "PriceReduce",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "AmountResidual",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "AmountResidualSigned",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "AmountTax",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "AmountTaxSigned",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "AmountTotal",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "AmountTotalSigned",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "AmountUntaxed",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "AmountUntaxedSigned",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "InvoiceOrigin",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "InvoicePaymentRef",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "InvoicePaymentState",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "InvoiceUserId",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AccountMoves");

            migrationBuilder.DropColumn(
                name: "AccountInternalType",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "ExcludeFromInvoiceTab",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "MoveName",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "ParentState",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "PriceSubtotal",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "PriceTotal",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "PriceUnit",
                table: "AccountMoveLines");
        }
    }
}
