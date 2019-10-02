using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LaboOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    SupplierId = table.Column<Guid>(nullable: false),
                    Color = table.Column<string>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: false),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    PriceSubtotal = table.Column<decimal>(nullable: false),
                    SentDate = table.Column<DateTime>(nullable: true),
                    ReceivedDate = table.Column<DateTime>(nullable: true),
                    WarrantyCode = table.Column<string>(nullable: true),
                    WarrantyPeriod = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboOrderLines_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrderLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrderLines_Partners_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrderLines_AccountInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrderLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrderLines_Partners_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrderLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_CompanyId",
                table: "LaboOrderLines",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_CreatedById",
                table: "LaboOrderLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_CustomerId",
                table: "LaboOrderLines",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_InvoiceId",
                table: "LaboOrderLines",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_ProductId",
                table: "LaboOrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_SupplierId",
                table: "LaboOrderLines",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_WriteById",
                table: "LaboOrderLines",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaboOrderLines");
        }
    }
}
