using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountRegisterPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    Communication = table.Column<string>(nullable: true),
                    JournalId = table.Column<Guid>(nullable: false),
                    PartnerType = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    PaymentType = table.Column<string>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRegisterPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountRegisterPayments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRegisterPayments_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRegisterPayments_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRegisterPayments_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountRegisterPaymentInvoiceRel",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRegisterPaymentInvoiceRel", x => new { x.PaymentId, x.InvoiceId });
                    table.ForeignKey(
                        name: "FK_AccountRegisterPaymentInvoiceRel_AccountInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountRegisterPaymentInvoiceRel_AccountRegisterPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "AccountRegisterPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRegisterPaymentInvoiceRel_InvoiceId",
                table: "AccountRegisterPaymentInvoiceRel",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRegisterPayments_CreatedById",
                table: "AccountRegisterPayments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRegisterPayments_JournalId",
                table: "AccountRegisterPayments",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRegisterPayments_PartnerId",
                table: "AccountRegisterPayments",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRegisterPayments_WriteById",
                table: "AccountRegisterPayments",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRegisterPaymentInvoiceRel");

            migrationBuilder.DropTable(
                name: "AccountRegisterPayments");
        }
    }
}
