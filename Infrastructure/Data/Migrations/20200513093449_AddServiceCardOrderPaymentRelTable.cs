using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddServiceCardOrderPaymentRelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceCardOrderPaymentRels",
                columns: table => new
                {
                    CardOrderId = table.Column<Guid>(nullable: false),
                    PaymentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCardOrderPaymentRels", x => new { x.PaymentId, x.CardOrderId });
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPaymentRels_ServiceCardOrders_CardOrderId",
                        column: x => x.CardOrderId,
                        principalTable: "ServiceCardOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPaymentRels_AccountPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "AccountPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPaymentRels_CardOrderId",
                table: "ServiceCardOrderPaymentRels",
                column: "CardOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceCardOrderPaymentRels");
        }
    }
}
