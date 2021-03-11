using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddStockInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryId",
                table: "StockMoves",
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "StockInventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    LocationId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: true),
                    CriteriaId = table.Column<Guid>(nullable: true),
                    Filter = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Exhausted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockInventory_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInventory_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockInventory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInventory_StockInventoryCriterias_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "StockInventoryCriterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInventory_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockInventory_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInventory_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockInventoryLine",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    LocationId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    ProductUOMId = table.Column<Guid>(nullable: false),
                    ProductQty = table.Column<decimal>(nullable: true),
                    TheoreticalQty = table.Column<decimal>(nullable: true),
                    InventoryId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Sequence = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInventoryLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockInventoryLine_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInventoryLine_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInventoryLine_StockInventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "StockInventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInventoryLine_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockInventoryLine_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockInventoryLine_UoMs_ProductUOMId",
                        column: x => x.ProductUOMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockInventoryLine_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_InventoryId",
                table: "StockMoves",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStockInventoryCriteriaRels_StockInventoryCriteriaId",
                table: "ProductStockInventoryCriteriaRels",
                column: "StockInventoryCriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventory_CategoryId",
                table: "StockInventory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventory_CompanyId",
                table: "StockInventory",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventory_CreatedById",
                table: "StockInventory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventory_CriteriaId",
                table: "StockInventory",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventory_LocationId",
                table: "StockInventory",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventory_ProductId",
                table: "StockInventory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventory_WriteById",
                table: "StockInventory",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryCriterias_CreatedById",
                table: "StockInventoryCriterias",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryCriterias_WriteById",
                table: "StockInventoryCriterias",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryLine_CompanyId",
                table: "StockInventoryLine",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryLine_CreatedById",
                table: "StockInventoryLine",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryLine_InventoryId",
                table: "StockInventoryLine",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryLine_LocationId",
                table: "StockInventoryLine",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryLine_ProductId",
                table: "StockInventoryLine",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryLine_ProductUOMId",
                table: "StockInventoryLine",
                column: "ProductUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventoryLine_WriteById",
                table: "StockInventoryLine",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_StockInventory_InventoryId",
                table: "StockMoves",
                column: "InventoryId",
                principalTable: "StockInventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMoves_StockInventory_InventoryId",
                table: "StockMoves");

            migrationBuilder.DropTable(
                name: "ProductStockInventoryCriteriaRels");

            migrationBuilder.DropTable(
                name: "StockInventoryLine");

            migrationBuilder.DropTable(
                name: "StockInventory");

            migrationBuilder.DropTable(
                name: "StockInventoryCriterias");

            migrationBuilder.DropIndex(
                name: "IX_StockMoves_InventoryId",
                table: "StockMoves");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "StockMoves");
        }
    }
}
