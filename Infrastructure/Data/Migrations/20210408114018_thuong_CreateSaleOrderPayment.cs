using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_CreateSaleOrderPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleOrderPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    MoveId = table.Column<Guid>(nullable: true),
                    PaymentMoveId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderPayments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPayments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPayments_AccountMoves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "AccountMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPayments_SaleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPayments_AccountMoves_PaymentMoveId",
                        column: x => x.PaymentMoveId,
                        principalTable: "AccountMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPayments_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderPaymentHistoryLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    SaleOrderLineId = table.Column<Guid>(nullable: false),
                    SaleOrderPaymentId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderPaymentHistoryLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentHistoryLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentHistoryLines_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentHistoryLines_SaleOrderPayments_SaleOrderPaymentId",
                        column: x => x.SaleOrderPaymentId,
                        principalTable: "SaleOrderPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentHistoryLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderPaymentJournalLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    JournalId = table.Column<Guid>(nullable: false),
                    SaleOrderPaymentId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderPaymentJournalLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentJournalLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentJournalLines_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentJournalLines_SaleOrderPayments_SaleOrderPaymentId",
                        column: x => x.SaleOrderPaymentId,
                        principalTable: "SaleOrderPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPaymentJournalLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentHistoryLines_CreatedById",
                table: "SaleOrderPaymentHistoryLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentHistoryLines_SaleOrderLineId",
                table: "SaleOrderPaymentHistoryLines",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentHistoryLines_SaleOrderPaymentId",
                table: "SaleOrderPaymentHistoryLines",
                column: "SaleOrderPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentHistoryLines_WriteById",
                table: "SaleOrderPaymentHistoryLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentJournalLines_CreatedById",
                table: "SaleOrderPaymentJournalLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentJournalLines_JournalId",
                table: "SaleOrderPaymentJournalLines",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentJournalLines_SaleOrderPaymentId",
                table: "SaleOrderPaymentJournalLines",
                column: "SaleOrderPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentJournalLines_WriteById",
                table: "SaleOrderPaymentJournalLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPayments_CompanyId",
                table: "SaleOrderPayments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPayments_CreatedById",
                table: "SaleOrderPayments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPayments_MoveId",
                table: "SaleOrderPayments",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPayments_OrderId",
                table: "SaleOrderPayments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPayments_PaymentMoveId",
                table: "SaleOrderPayments",
                column: "PaymentMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPayments_WriteById",
                table: "SaleOrderPayments",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderPaymentHistoryLines");

            migrationBuilder.DropTable(
                name: "SaleOrderPaymentJournalLines");

            migrationBuilder.DropTable(
                name: "SaleOrderPayments");
        }
    }
}
