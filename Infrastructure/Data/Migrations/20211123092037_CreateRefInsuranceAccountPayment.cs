using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateRefInsuranceAccountPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "InsuranceId",
                table: "SaleOrderPaymentJournalLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InsuranceId",
                table: "AccountPayments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentJournalLines_InsuranceId",
                table: "SaleOrderPaymentJournalLines",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayments_InsuranceId",
                table: "AccountPayments",
                column: "InsuranceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPayments_ResInsurances_InsuranceId",
                table: "AccountPayments",
                column: "InsuranceId",
                principalTable: "ResInsurances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderPaymentJournalLines_ResInsurances_InsuranceId",
                table: "SaleOrderPaymentJournalLines",
                column: "InsuranceId",
                principalTable: "ResInsurances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountPayments_ResInsurances_InsuranceId",
                table: "AccountPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderPaymentJournalLines_ResInsurances_InsuranceId",
                table: "SaleOrderPaymentJournalLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderPaymentJournalLines_InsuranceId",
                table: "SaleOrderPaymentJournalLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountPayments_InsuranceId",
                table: "AccountPayments");

            migrationBuilder.DropColumn(
                name: "InsuranceId",
                table: "SaleOrderPaymentJournalLines");

            migrationBuilder.DropColumn(
                name: "InsuranceId",
                table: "AccountPayments");

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "SaleOrderPaymentJournalLines",
                type: "uniqueidentifier",
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
    }
}
