using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddServiceCardOrderPaymentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountMoveId",
                table: "ServiceCardOrders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceCardOrderPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    JournalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCardOrderPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPayments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPayments_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPayments_ServiceCardOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ServiceCardOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPayments_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_AccountMoveId",
                table: "ServiceCardOrders",
                column: "AccountMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPayments_CreatedById",
                table: "ServiceCardOrderPayments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPayments_JournalId",
                table: "ServiceCardOrderPayments",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPayments_OrderId",
                table: "ServiceCardOrderPayments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPayments_WriteById",
                table: "ServiceCardOrderPayments",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCardOrders_AccountMoves_AccountMoveId",
                table: "ServiceCardOrders",
                column: "AccountMoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCardOrders_AccountMoves_AccountMoveId",
                table: "ServiceCardOrders");

            migrationBuilder.DropTable(
                name: "ServiceCardOrderPayments");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCardOrders_AccountMoveId",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "AccountMoveId",
                table: "ServiceCardOrders");
        }
    }
}
