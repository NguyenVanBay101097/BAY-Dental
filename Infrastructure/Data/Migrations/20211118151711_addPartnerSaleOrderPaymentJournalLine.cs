using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class addPartnerSaleOrderPaymentJournalLine : Migration
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
                table: "SaleOrderPaymentJournalLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentJournalLines_PartnerId",
                table: "SaleOrderPaymentJournalLines",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderPaymentJournalLines_Partners_PartnerId",
                table: "SaleOrderPaymentJournalLines",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderPaymentJournalLines_Partners_PartnerId",
                table: "SaleOrderPaymentJournalLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderPaymentJournalLines_PartnerId",
                table: "SaleOrderPaymentJournalLines");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "SaleOrderPaymentJournalLines");

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
