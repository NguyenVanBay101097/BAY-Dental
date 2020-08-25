using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddSaleOrderLineCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SaleOrderLinePaymentRels",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerCommissionId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountPrepaid",
                table: "SaleOrderLinePaymentRels",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "SaleOrderLinePaymentRels",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "SaleOrderLinePaymentRels",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "SaleOrderLinePaymentRels",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "SaleOrderLinePaymentRels",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WriteById",
                table: "SaleOrderLinePaymentRels",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SaleOrderLinePaymentRels",
                table: "SaleOrderLinePaymentRels",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_PartnerCommissionId",
                table: "SaleOrderLines",
                column: "PartnerCommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePaymentRels_CreatedById",
                table: "SaleOrderLinePaymentRels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePaymentRels_PaymentId",
                table: "SaleOrderLinePaymentRels",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePaymentRels_WriteById",
                table: "SaleOrderLinePaymentRels",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLinePaymentRels_AspNetUsers_CreatedById",
                table: "SaleOrderLinePaymentRels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLinePaymentRels_AspNetUsers_WriteById",
                table: "SaleOrderLinePaymentRels",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_SaleOrderLinePartnerCommissions_PartnerCommissionId",
                table: "SaleOrderLines",
                column: "PartnerCommissionId",
                principalTable: "SaleOrderLinePartnerCommissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLinePaymentRels_AspNetUsers_CreatedById",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLinePaymentRels_AspNetUsers_WriteById",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_SaleOrderLinePartnerCommissions_PartnerCommissionId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_PartnerCommissionId",
                table: "SaleOrderLines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SaleOrderLinePaymentRels",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLinePaymentRels_CreatedById",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLinePaymentRels_PaymentId",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLinePaymentRels_WriteById",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropColumn(
                name: "PartnerCommissionId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.DropColumn(
                name: "WriteById",
                table: "SaleOrderLinePaymentRels");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountPrepaid",
                table: "SaleOrderLinePaymentRels",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SaleOrderLinePaymentRels",
                table: "SaleOrderLinePaymentRels",
                columns: new[] { "PaymentId", "SaleOrderLineId" });
        }
    }
}
