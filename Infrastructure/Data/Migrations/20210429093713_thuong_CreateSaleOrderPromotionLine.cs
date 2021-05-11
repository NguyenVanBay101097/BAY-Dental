using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_CreateSaleOrderPromotionLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderPromotions_SaleOrderPromotions_ParentId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderPromotions_Products_ProductId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderPromotions_ParentId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderPromotions_ProductId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "SaleOrderPromotions");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "SaleOrderPromotions");

            migrationBuilder.CreateTable(
                name: "SaleOrderPromotionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    SaleOrderLineId = table.Column<Guid>(nullable: false),
                    PromotionId = table.Column<Guid>(nullable: false),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderPromotionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotionLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotionLines_SaleOrderPromotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "SaleOrderPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotionLines_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotionLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotionLines_CreatedById",
                table: "SaleOrderPromotionLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotionLines_PromotionId",
                table: "SaleOrderPromotionLines",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotionLines_SaleOrderLineId",
                table: "SaleOrderPromotionLines",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotionLines_WriteById",
                table: "SaleOrderPromotionLines",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderPromotionLines");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "SaleOrderPromotions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "SaleOrderPromotions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_ParentId",
                table: "SaleOrderPromotions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_ProductId",
                table: "SaleOrderPromotions",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderPromotions_SaleOrderPromotions_ParentId",
                table: "SaleOrderPromotions",
                column: "ParentId",
                principalTable: "SaleOrderPromotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderPromotions_Products_ProductId",
                table: "SaleOrderPromotions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
