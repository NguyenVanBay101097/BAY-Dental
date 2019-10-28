using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F70 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceStatus",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceStatus",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QtyInvoiced",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QtyToInvoice",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SaleOrderLineInvoiceRels",
                columns: table => new
                {
                    OrderLineId = table.Column<Guid>(nullable: false),
                    InvoiceLineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLineInvoiceRels", x => new { x.OrderLineId, x.InvoiceLineId });
                    table.ForeignKey(
                        name: "FK_SaleOrderLineInvoiceRels_AccountInvoiceLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "AccountInvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLineInvoiceRels_SaleOrderLines_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineInvoiceRels_InvoiceLineId",
                table: "SaleOrderLineInvoiceRels",
                column: "InvoiceLineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderLineInvoiceRels");

            migrationBuilder.DropColumn(
                name: "InvoiceStatus",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "InvoiceStatus",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "QtyInvoiced",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "QtyToInvoice",
                table: "SaleOrderLines");
        }
    }
}
