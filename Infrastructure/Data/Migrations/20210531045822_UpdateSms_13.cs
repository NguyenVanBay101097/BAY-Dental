using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSms_13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SmsConfigs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmsConfigProductCategoryRels",
                columns: table => new
                {
                    SmsConfigId = table.Column<Guid>(nullable: false),
                    ProductCategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsConfigProductCategoryRels", x => new { x.SmsConfigId, x.ProductCategoryId });
                    table.ForeignKey(
                        name: "FK_SmsConfigProductCategoryRels_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsConfigProductCategoryRels_SmsConfigs_SmsConfigId",
                        column: x => x.SmsConfigId,
                        principalTable: "SmsConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmsConfigProductRels",
                columns: table => new
                {
                    SmsConfigId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsConfigProductRels", x => new { x.SmsConfigId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_SmsConfigProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsConfigProductRels_SmsConfigs_SmsConfigId",
                        column: x => x.SmsConfigId,
                        principalTable: "SmsConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigProductCategoryRels_ProductCategoryId",
                table: "SmsConfigProductCategoryRels",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigProductRels_ProductId",
                table: "SmsConfigProductRels",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmsConfigProductCategoryRels");

            migrationBuilder.DropTable(
                name: "SmsConfigProductRels");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SmsConfigs");
        }
    }
}
