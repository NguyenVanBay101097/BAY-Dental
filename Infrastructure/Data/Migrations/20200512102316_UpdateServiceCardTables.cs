using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateServiceCardTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCardCards_ServiceCardOrders_OrderId",
                table: "ServiceCardCards");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCardOrders_ServiceCardTypes_CardTypeId",
                table: "ServiceCardOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCardOrders_AccountMoves_MoveId",
                table: "ServiceCardOrders");

            migrationBuilder.DropTable(
                name: "ServiceCardOrderPartnerRels");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCardOrders_CardTypeId",
                table: "ServiceCardOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCardOrders_MoveId",
                table: "ServiceCardOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCardCards_OrderId",
                table: "ServiceCardCards");

            migrationBuilder.DropColumn(
                name: "ActivatedDate",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "CardTypeId",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "GenerationType",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "MoveId",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "PriceUnit",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ServiceCardCards");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountResidual",
                table: "ServiceCardOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleLineId",
                table: "ServiceCardCards",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceCardOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    ProductUOMQty = table.Column<decimal>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    OrderPartnerId = table.Column<Guid>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    CardTypeId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    PriceSubTotal = table.Column<decimal>(nullable: false),
                    PriceTotal = table.Column<decimal>(nullable: false),
                    SalesmanId = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    DiscountType = table.Column<string>(nullable: true),
                    DiscountFixed = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCardOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderLines_ServiceCardTypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "ServiceCardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderLines_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderLines_ServiceCardOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ServiceCardOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderLines_Partners_OrderPartnerId",
                        column: x => x.OrderPartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderLines_AspNetUsers_SalesmanId",
                        column: x => x.SalesmanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCardOrderLineInvoiceRels",
                columns: table => new
                {
                    OrderLineId = table.Column<Guid>(nullable: false),
                    InvoiceLineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCardOrderLineInvoiceRels", x => new { x.OrderLineId, x.InvoiceLineId });
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderLineInvoiceRels_AccountMoveLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "AccountMoveLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderLineInvoiceRels_ServiceCardOrderLines_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "ServiceCardOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardCards_SaleLineId",
                table: "ServiceCardCards",
                column: "SaleLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderLineInvoiceRels_InvoiceLineId",
                table: "ServiceCardOrderLineInvoiceRels",
                column: "InvoiceLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderLines_CardTypeId",
                table: "ServiceCardOrderLines",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderLines_CompanyId",
                table: "ServiceCardOrderLines",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderLines_CreatedById",
                table: "ServiceCardOrderLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderLines_OrderId",
                table: "ServiceCardOrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderLines_OrderPartnerId",
                table: "ServiceCardOrderLines",
                column: "OrderPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderLines_SalesmanId",
                table: "ServiceCardOrderLines",
                column: "SalesmanId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderLines_WriteById",
                table: "ServiceCardOrderLines",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCardCards_ServiceCardOrderLines_SaleLineId",
                table: "ServiceCardCards",
                column: "SaleLineId",
                principalTable: "ServiceCardOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCardCards_ServiceCardOrderLines_SaleLineId",
                table: "ServiceCardCards");

            migrationBuilder.DropTable(
                name: "ServiceCardOrderLineInvoiceRels");

            migrationBuilder.DropTable(
                name: "ServiceCardOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCardCards_SaleLineId",
                table: "ServiceCardCards");

            migrationBuilder.DropColumn(
                name: "AmountResidual",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "SaleLineId",
                table: "ServiceCardCards");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivatedDate",
                table: "ServiceCardOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CardTypeId",
                table: "ServiceCardOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "GenerationType",
                table: "ServiceCardOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MoveId",
                table: "ServiceCardOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceUnit",
                table: "ServiceCardOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "ServiceCardOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "ServiceCardCards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceCardOrderPartnerRels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WriteById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCardOrderPartnerRels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPartnerRels_ServiceCardOrders_CardOrderId",
                        column: x => x.CardOrderId,
                        principalTable: "ServiceCardOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPartnerRels_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPartnerRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPartnerRels_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_CardTypeId",
                table: "ServiceCardOrders",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_MoveId",
                table: "ServiceCardOrders",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardCards_OrderId",
                table: "ServiceCardCards",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPartnerRels_CardOrderId",
                table: "ServiceCardOrderPartnerRels",
                column: "CardOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPartnerRels_CreatedById",
                table: "ServiceCardOrderPartnerRels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPartnerRels_PartnerId",
                table: "ServiceCardOrderPartnerRels",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPartnerRels_WriteById",
                table: "ServiceCardOrderPartnerRels",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCardCards_ServiceCardOrders_OrderId",
                table: "ServiceCardCards",
                column: "OrderId",
                principalTable: "ServiceCardOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCardOrders_ServiceCardTypes_CardTypeId",
                table: "ServiceCardOrders",
                column: "CardTypeId",
                principalTable: "ServiceCardTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCardOrders_AccountMoves_MoveId",
                table: "ServiceCardOrders",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
