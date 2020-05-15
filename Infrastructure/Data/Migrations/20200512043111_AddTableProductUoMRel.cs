using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTableProductUoMRel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductUoMRels",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(nullable: false),
                    UoMId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUoMRels", x => new { x.UoMId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ProductUoMRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductUoMRels_UoMs_UoMId",
                        column: x => x.UoMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductUoMRels_ProductId",
                table: "ProductUoMRels",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductUoMRels");
        }
    }
}
