using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class bay_add_requestedproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleOrderLineProductRequesteds",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    SaleOrderLineId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    RequestedQuantity = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLineProductRequesteds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderLineProductRequesteds_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLineProductRequesteds_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLineProductRequesteds_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLineProductRequesteds_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineProductRequesteds_CreatedById",
                table: "SaleOrderLineProductRequesteds",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineProductRequesteds_ProductId",
                table: "SaleOrderLineProductRequesteds",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineProductRequesteds_SaleOrderLineId",
                table: "SaleOrderLineProductRequesteds",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineProductRequesteds_WriteById",
                table: "SaleOrderLineProductRequesteds",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderLineProductRequesteds");
        }
    }
}
