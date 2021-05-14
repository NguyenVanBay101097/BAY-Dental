using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_ChangeCommissionSettlement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Partners_PartnerId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_AccountPayments_PaymentId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_SaleOrders_SaleOrderId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_SaleOrderLines_SaleOrderLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLinePartnerCommissions_Partners_PartnerId",
                table: "SaleOrderLinePartnerCommissions");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLinePartnerCommissions_PartnerId",
                table: "SaleOrderLinePartnerCommissions");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_PartnerId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_PaymentId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_SaleOrderId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_SaleOrderLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "SaleOrderLinePartnerCommissions");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "SaleOrderLinePartnerCommissions");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "SaleOrderLinePartnerCommissions");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "SaleOrderId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "SaleOrderLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "PercentAdvisory",
                table: "CommissionProductRules");

            migrationBuilder.DropColumn(
                name: "PercentAssistant",
                table: "CommissionProductRules");

            migrationBuilder.DropColumn(
                name: "PercentDoctor",
                table: "CommissionProductRules");

            migrationBuilder.AddColumn<Guid>(
                name: "AssistantCommissionId",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CounselorCommissionId",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Commissions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Percent",
                table: "CommissionProductRules",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_AssistantCommissionId",
                table: "Employees",
                column: "AssistantCommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CounselorCommissionId",
                table: "Employees",
                column: "CounselorCommissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Commissions_AssistantCommissionId",
                table: "Employees",
                column: "AssistantCommissionId",
                principalTable: "Commissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Commissions_CounselorCommissionId",
                table: "Employees",
                column: "CounselorCommissionId",
                principalTable: "Commissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Commissions_AssistantCommissionId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Commissions_CounselorCommissionId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_AssistantCommissionId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CounselorCommissionId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AssistantCommissionId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CounselorCommissionId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Commissions");

            migrationBuilder.DropColumn(
                name: "Percent",
                table: "CommissionProductRules");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "SaleOrderLinePartnerCommissions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "SaleOrderLinePartnerCommissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Percentage",
                table: "SaleOrderLinePartnerCommissions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "CommissionSettlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "CommissionSettlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderId",
                table: "CommissionSettlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderLineId",
                table: "CommissionSettlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CommissionSettlements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentAdvisory",
                table: "CommissionProductRules",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentAssistant",
                table: "CommissionProductRules",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentDoctor",
                table: "CommissionProductRules",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePartnerCommissions_PartnerId",
                table: "SaleOrderLinePartnerCommissions",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_PartnerId",
                table: "CommissionSettlements",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_PaymentId",
                table: "CommissionSettlements",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_SaleOrderId",
                table: "CommissionSettlements",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_SaleOrderLineId",
                table: "CommissionSettlements",
                column: "SaleOrderLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_Partners_PartnerId",
                table: "CommissionSettlements",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_AccountPayments_PaymentId",
                table: "CommissionSettlements",
                column: "PaymentId",
                principalTable: "AccountPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_SaleOrders_SaleOrderId",
                table: "CommissionSettlements",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_SaleOrderLines_SaleOrderLineId",
                table: "CommissionSettlements",
                column: "SaleOrderLineId",
                principalTable: "SaleOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLinePartnerCommissions_Partners_PartnerId",
                table: "SaleOrderLinePartnerCommissions",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
