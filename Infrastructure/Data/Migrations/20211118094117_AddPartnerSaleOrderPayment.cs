using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddPartnerSaleOrderPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResInsurancePayments_AccountMoves_MoveId",
                table: "ResInsurancePayments");

            migrationBuilder.DropIndex(
                name: "IX_ResInsurancePayments_MoveId",
                table: "ResInsurancePayments");

            migrationBuilder.DropColumn(
                name: "MoveId",
                table: "ResInsurancePayments");

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "SaleOrderPayments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPayments_PartnerId",
                table: "SaleOrderPayments",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderPayments_Partners_PartnerId",
                table: "SaleOrderPayments",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderPayments_Partners_PartnerId",
                table: "SaleOrderPayments");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderPayments_PartnerId",
                table: "SaleOrderPayments");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "SaleOrderPayments");

            migrationBuilder.AddColumn<Guid>(
                name: "MoveId",
                table: "ResInsurancePayments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePayments_MoveId",
                table: "ResInsurancePayments",
                column: "MoveId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResInsurancePayments_AccountMoves_MoveId",
                table: "ResInsurancePayments",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
