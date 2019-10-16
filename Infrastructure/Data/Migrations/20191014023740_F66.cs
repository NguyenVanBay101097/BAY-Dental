using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F66 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductPricelists",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateEnd = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPricelists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPricelists_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelists_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelists_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductPricelistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    CategId = table.Column<Guid>(nullable: true),
                    AppliedOn = table.Column<string>(nullable: true),
                    MinQuantity = table.Column<int>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    Base = table.Column<string>(nullable: true),
                    PriceListId = table.Column<Guid>(nullable: true),
                    PriceSurcharge = table.Column<decimal>(nullable: true),
                    PriceDiscount = table.Column<decimal>(nullable: true),
                    PriceRound = table.Column<decimal>(nullable: true),
                    PriceMinMargin = table.Column<decimal>(nullable: true),
                    PriceMaxMargin = table.Column<decimal>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateEnd = table.Column<DateTime>(nullable: true),
                    ComputePrice = table.Column<string>(nullable: true),
                    FixedPrice = table.Column<decimal>(nullable: true),
                    PercentPrice = table.Column<decimal>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPricelistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_ProductCategories_CategId",
                        column: x => x.CategId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_ProductPricelists_PriceListId",
                        column: x => x.PriceListId,
                        principalTable: "ProductPricelists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_CategId",
                table: "ProductPricelistItems",
                column: "CategId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_CompanyId",
                table: "ProductPricelistItems",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_CreatedById",
                table: "ProductPricelistItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_PriceListId",
                table: "ProductPricelistItems",
                column: "PriceListId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_ProductId",
                table: "ProductPricelistItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_WriteById",
                table: "ProductPricelistItems",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_CompanyId",
                table: "ProductPricelists",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_CreatedById",
                table: "ProductPricelists",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_WriteById",
                table: "ProductPricelists",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPricelistItems");

            migrationBuilder.DropTable(
                name: "ProductPricelists");
        }
    }
}
