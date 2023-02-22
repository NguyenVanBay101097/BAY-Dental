using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateCommissionSettlement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommissionSettlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    BaseAmount = table.Column<decimal>(nullable: true),
                    Percentage = table.Column<decimal>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    SaleOrderLineId = table.Column<Guid>(nullable: true),
                    PaymentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionSettlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommissionSettlements_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionSettlements_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionSettlements_AccountPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "AccountPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionSettlements_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionSettlements_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_CreatedById",
                table: "CommissionSettlements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_PartnerId",
                table: "CommissionSettlements",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_PaymentId",
                table: "CommissionSettlements",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_SaleOrderLineId",
                table: "CommissionSettlements",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_WriteById",
                table: "CommissionSettlements",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommissionSettlements");
        }
    }
}
