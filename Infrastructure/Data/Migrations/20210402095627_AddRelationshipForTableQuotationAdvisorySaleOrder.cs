using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddRelationshipForTableQuotationAdvisorySaleOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuotationId",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdvisoryId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdvisoryId",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_QuotationId",
                table: "SaleOrders",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_AdvisoryId",
                table: "SaleOrderLines",
                column: "AdvisoryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_AdvisoryId",
                table: "QuotationLines",
                column: "AdvisoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_Advisory_AdvisoryId",
                table: "QuotationLines",
                column: "AdvisoryId",
                principalTable: "Advisory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_Advisory_AdvisoryId",
                table: "SaleOrderLines",
                column: "AdvisoryId",
                principalTable: "Advisory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_Quotations_QuotationId",
                table: "SaleOrders",
                column: "QuotationId",
                principalTable: "Quotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_Advisory_AdvisoryId",
                table: "QuotationLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_Advisory_AdvisoryId",
                table: "SaleOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_Quotations_QuotationId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_QuotationId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_AdvisoryId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_AdvisoryId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "QuotationId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "AdvisoryId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "AdvisoryId",
                table: "QuotationLines");
        }
    }
}
