using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F74 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Diagnostic",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToothCategoryId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SaleOrderLineToothRels",
                columns: table => new
                {
                    SaleLineId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLineToothRels", x => new { x.SaleLineId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_SaleOrderLineToothRels_SaleOrderLines_SaleLineId",
                        column: x => x.SaleLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLineToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_ToothCategoryId",
                table: "SaleOrderLines",
                column: "ToothCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineToothRels_ToothId",
                table: "SaleOrderLineToothRels",
                column: "ToothId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_ToothCategories_ToothCategoryId",
                table: "SaleOrderLines",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_ToothCategories_ToothCategoryId",
                table: "SaleOrderLines");

            migrationBuilder.DropTable(
                name: "SaleOrderLineToothRels");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_ToothCategoryId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "Diagnostic",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "ToothCategoryId",
                table: "SaleOrderLines");
        }
    }
}
