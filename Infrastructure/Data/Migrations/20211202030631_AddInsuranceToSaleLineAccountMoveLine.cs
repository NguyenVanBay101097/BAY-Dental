using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddInsuranceToSaleLineAccountMoveLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InsuranceId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InsuranceId",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_InsuranceId",
                table: "SaleOrderLines",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_InsuranceId",
                table: "AccountMoveLines",
                column: "InsuranceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_ResInsurances_InsuranceId",
                table: "AccountMoveLines",
                column: "InsuranceId",
                principalTable: "ResInsurances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_ResInsurances_InsuranceId",
                table: "SaleOrderLines",
                column: "InsuranceId",
                principalTable: "ResInsurances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_ResInsurances_InsuranceId",
                table: "AccountMoveLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_ResInsurances_InsuranceId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_InsuranceId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_InsuranceId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "InsuranceId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "InsuranceId",
                table: "AccountMoveLines");
        }
    }
}
