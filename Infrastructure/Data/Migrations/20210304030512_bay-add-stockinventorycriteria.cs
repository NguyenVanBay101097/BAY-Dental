using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class bayaddstockinventorycriteria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMoves_StockInventory_StockInventoryId",
                table: "StockMoves");

            migrationBuilder.DropIndex(
                name: "IX_StockMoves_StockInventoryId",
                table: "StockMoves");

            migrationBuilder.DropColumn(
                name: "StockInventoryId",
                table: "StockMoves");

            migrationBuilder.CreateTable(
                name: "StockInventoryCriterias",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInventoryCriterias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockInventoryCriterias_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInventoryCriterias_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductStockInventoryCriteriaRels",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(nullable: false),
                    StockInventoryCriteriaId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStockInventoryCriteriaRels", x => new { x.ProductId, x.StockInventoryCriteriaId });
                    table.ForeignKey(
                        name: "FK_ProductStockInventoryCriteriaRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductStockInventoryCriteriaRels_StockInventoryCriterias_StockInventoryCriteriaId",
                        column: x => x.StockInventoryCriteriaId,
                        principalTable: "StockInventoryCriterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductStockInventoryCriteriaRels_StockInventoryCriteriaId",
                table: "ProductStockInventoryCriteriaRels",
                column: "StockInventoryCriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryCriterias_CreatedById",
                table: "StockInventoryCriterias",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryCriterias_WriteById",
                table: "StockInventoryCriterias",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductStockInventoryCriteriaRels");

            migrationBuilder.DropTable(
                name: "StockInventoryCriterias");

            migrationBuilder.AddColumn<Guid>(
                name: "StockInventoryId",
                table: "StockMoves",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_StockInventoryId",
                table: "StockMoves",
                column: "StockInventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_StockInventory_StockInventoryId",
                table: "StockMoves",
                column: "StockInventoryId",
                principalTable: "StockInventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
