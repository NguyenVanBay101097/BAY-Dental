﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddSaleOrderPaymentRelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleOrderPaymentRels",
                columns: table => new
                {
                    SaleOrderId = table.Column<Guid>(nullable: false),
                    PaymentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderPaymentRels", x => new { x.PaymentId, x.SaleOrderId });
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentRels_AccountPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "AccountPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentRels_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentRels_SaleOrderId",
                table: "SaleOrderPaymentRels",
                column: "SaleOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderPaymentRels");
        }
    }
}
