using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F81 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseLineId",
                table: "AccountInvoiceLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    PartnerRef = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    DateOrder = table.Column<DateTime>(nullable: false),
                    DateApprove = table.Column<DateTime>(nullable: true),
                    PickingTypeId = table.Column<Guid>(nullable: false),
                    AmountTotal = table.Column<decimal>(nullable: true),
                    AmountUntaxed = table.Column<decimal>(nullable: true),
                    AmountTax = table.Column<decimal>(nullable: true),
                    Origin = table.Column<string>(nullable: true),
                    DatePlanned = table.Column<DateTime>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    InvoiceStatus = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    RefundOrderId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_StockPickingTypes_PickingTypeId",
                        column: x => x.PickingTypeId,
                        principalTable: "StockPickingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_PurchaseOrders_RefundOrderId",
                        column: x => x.RefundOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    ProductQty = table.Column<decimal>(nullable: false),
                    ProductUOMQty = table.Column<decimal>(nullable: true),
                    ProductUOMId = table.Column<Guid>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    PriceSubtotal = table.Column<decimal>(nullable: true),
                    PriceTotal = table.Column<decimal>(nullable: true),
                    PriceTax = table.Column<decimal>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    DatePlanned = table.Column<DateTime>(nullable: true),
                    Discount = table.Column<decimal>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    QtyInvoiced = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_PurchaseOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_UoMs_ProductUOMId",
                        column: x => x.ProductUOMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_PurchaseLineId",
                table: "AccountInvoiceLines",
                column: "PurchaseLineId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_CompanyId",
                table: "PurchaseOrderLines",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_CreatedById",
                table: "PurchaseOrderLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_OrderId",
                table: "PurchaseOrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PartnerId",
                table: "PurchaseOrderLines",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_ProductId",
                table: "PurchaseOrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_ProductUOMId",
                table: "PurchaseOrderLines",
                column: "ProductUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_WriteById",
                table: "PurchaseOrderLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CompanyId",
                table: "PurchaseOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CreatedById",
                table: "PurchaseOrders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PartnerId",
                table: "PurchaseOrders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PickingTypeId",
                table: "PurchaseOrders",
                column: "PickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_RefundOrderId",
                table: "PurchaseOrders",
                column: "RefundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_UserId",
                table: "PurchaseOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_WriteById",
                table: "PurchaseOrders",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_PurchaseOrderLines_PurchaseLineId",
                table: "AccountInvoiceLines",
                column: "PurchaseLineId",
                principalTable: "PurchaseOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoiceLines_PurchaseOrderLines_PurchaseLineId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropTable(
                name: "PurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_AccountInvoiceLines_PurchaseLineId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropColumn(
                name: "PurchaseLineId",
                table: "AccountInvoiceLines");
        }
    }
}
