using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddSaleOrderServiceCardCardRelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleOrderServiceCardCardRels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: false),
                    CardId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderServiceCardCardRels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderServiceCardCardRels_ServiceCardCards_CardId",
                        column: x => x.CardId,
                        principalTable: "ServiceCardCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderServiceCardCardRels_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderServiceCardCardRels_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderServiceCardCardRels_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderServiceCardCardRels_CardId",
                table: "SaleOrderServiceCardCardRels",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderServiceCardCardRels_CreatedById",
                table: "SaleOrderServiceCardCardRels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderServiceCardCardRels_SaleOrderId",
                table: "SaleOrderServiceCardCardRels",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderServiceCardCardRels_WriteById",
                table: "SaleOrderServiceCardCardRels",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderServiceCardCardRels");
        }
    }
}
