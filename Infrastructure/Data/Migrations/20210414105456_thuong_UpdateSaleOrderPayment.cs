using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_UpdateSaleOrderPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_SaleOrderPayments_SaleOrderPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_SaleOrderPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "SaleOrderPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.CreateTable(
                name: "SaleOrderPaymentAccountPaymentRels",
                columns: table => new
                {
                    SaleOrderPaymentId = table.Column<Guid>(nullable: false),
                    PaymentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderPaymentAccountPaymentRels", x => new { x.SaleOrderPaymentId, x.PaymentId });
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentAccountPaymentRels_AccountPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "AccountPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentAccountPaymentRels_SaleOrderPayments_SaleOrderPaymentId",
                        column: x => x.SaleOrderPaymentId,
                        principalTable: "SaleOrderPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentAccountPaymentRels_PaymentId",
                table: "SaleOrderPaymentAccountPaymentRels",
                column: "PaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderPaymentAccountPaymentRels");

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderPaymentId",
                table: "AccountMoveLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_SaleOrderPaymentId",
                table: "AccountMoveLines",
                column: "SaleOrderPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_SaleOrderPayments_SaleOrderPaymentId",
                table: "AccountMoveLines",
                column: "SaleOrderPaymentId",
                principalTable: "SaleOrderPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
