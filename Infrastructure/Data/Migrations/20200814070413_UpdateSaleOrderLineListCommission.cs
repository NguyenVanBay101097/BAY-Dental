using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSaleOrderLineListCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_SaleOrderLinePartnerCommissions_PartnerCommissionId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_PartnerCommissionId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "PartnerCommissionId",
                table: "SaleOrderLines");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "SaleOrderLinePartnerCommissions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "SaleOrderLinePartnerCommissions");

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerCommissionId",
                table: "SaleOrderLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_PartnerCommissionId",
                table: "SaleOrderLines",
                column: "PartnerCommissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_SaleOrderLinePartnerCommissions_PartnerCommissionId",
                table: "SaleOrderLines",
                column: "PartnerCommissionId",
                principalTable: "SaleOrderLinePartnerCommissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
