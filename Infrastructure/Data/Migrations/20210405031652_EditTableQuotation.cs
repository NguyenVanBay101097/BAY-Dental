using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class EditTableQuotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentDiscount",
                table: "QuotationLines");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Quotations",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CompanyId",
                table: "Quotations",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Companies_CompanyId",
                table: "Quotations",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Companies_CompanyId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_CompanyId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "QuotationLines");

            migrationBuilder.AddColumn<int>(
                name: "PercentDiscount",
                table: "QuotationLines",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
