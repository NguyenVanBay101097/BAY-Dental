using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class insertUOMintoQuotationLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductUOMId",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_ProductUOMId",
                table: "QuotationLines",
                column: "ProductUOMId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_UoMs_ProductUOMId",
                table: "QuotationLines",
                column: "ProductUOMId",
                principalTable: "UoMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_UoMs_ProductUOMId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_ProductUOMId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "ProductUOMId",
                table: "QuotationLines");
        }
    }
}
