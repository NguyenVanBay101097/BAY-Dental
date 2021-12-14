using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateSaleProduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleProductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleProductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleProductions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleProductions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleProductions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleProductions_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderLineSaleProductionRels",
                columns: table => new
                {
                    OrderLineId = table.Column<Guid>(nullable: false),
                    SaleProductionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLineSaleProductionRels", x => new { x.OrderLineId, x.SaleProductionId });
                    table.ForeignKey(
                        name: "FK_SaleOrderLineSaleProductionRels_SaleOrderLines_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLineSaleProductionRels_SaleProductions_SaleProductionId",
                        column: x => x.SaleProductionId,
                        principalTable: "SaleProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleProductionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    SaleProductionId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    QuantityRequested = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleProductionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleProductionLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleProductionLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleProductionLines_SaleProductions_SaleProductionId",
                        column: x => x.SaleProductionId,
                        principalTable: "SaleProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleProductionLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleProductionLineProductRequestLineRels",
                columns: table => new
                {
                    SaleProductionLineId = table.Column<Guid>(nullable: false),
                    ProductRequestLineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleProductionLineProductRequestLineRels", x => new { x.SaleProductionLineId, x.ProductRequestLineId });
                    table.ForeignKey(
                        name: "FK_SaleProductionLineProductRequestLineRels_ProductRequestLines_ProductRequestLineId",
                        column: x => x.ProductRequestLineId,
                        principalTable: "ProductRequestLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleProductionLineProductRequestLineRels_SaleProductionLines_SaleProductionLineId",
                        column: x => x.SaleProductionLineId,
                        principalTable: "SaleProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineSaleProductionRels_SaleProductionId",
                table: "SaleOrderLineSaleProductionRels",
                column: "SaleProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductionLineProductRequestLineRels_ProductRequestLineId",
                table: "SaleProductionLineProductRequestLineRels",
                column: "ProductRequestLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductionLines_CreatedById",
                table: "SaleProductionLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductionLines_ProductId",
                table: "SaleProductionLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductionLines_SaleProductionId",
                table: "SaleProductionLines",
                column: "SaleProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductionLines_WriteById",
                table: "SaleProductionLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductions_CompanyId",
                table: "SaleProductions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductions_CreatedById",
                table: "SaleProductions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductions_ProductId",
                table: "SaleProductions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductions_WriteById",
                table: "SaleProductions",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderLineSaleProductionRels");

            migrationBuilder.DropTable(
                name: "SaleProductionLineProductRequestLineRels");

            migrationBuilder.DropTable(
                name: "SaleProductionLines");

            migrationBuilder.DropTable(
                name: "SaleProductions");
        }
    }
}
