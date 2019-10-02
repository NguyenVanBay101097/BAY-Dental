using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountAccountTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    IncludeInitialBalance = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAccountTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountAccountTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountAccountTypes_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    UserTypeId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    InternalType = table.Column<string>(nullable: true),
                    Reconcile = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountAccounts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountAccounts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountAccounts_AccountAccountTypes_UserTypeId",
                        column: x => x.UserTypeId,
                        principalTable: "AccountAccountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountAccounts_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountJournals",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    DefaultDebitAccountId = table.Column<Guid>(nullable: true),
                    DefaultCreditAccountId = table.Column<Guid>(nullable: true),
                    UpdatePosted = table.Column<bool>(nullable: false),
                    SequenceId = table.Column<Guid>(nullable: false),
                    RefundSequenceId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    DedicatedRefund = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountJournals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountJournals_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountJournals_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountJournals_AccountAccounts_DefaultCreditAccountId",
                        column: x => x.DefaultCreditAccountId,
                        principalTable: "AccountAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountJournals_AccountAccounts_DefaultDebitAccountId",
                        column: x => x.DefaultDebitAccountId,
                        principalTable: "AccountAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountJournals_IRSequences_RefundSequenceId",
                        column: x => x.RefundSequenceId,
                        principalTable: "IRSequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountJournals_IRSequences_SequenceId",
                        column: x => x.SequenceId,
                        principalTable: "IRSequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountJournals_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountMoves",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Ref = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    JournalId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: false),
                    Narration = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountMoves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountMoves_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoves_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoves_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoves_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoves_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    PartnerType = table.Column<string>(nullable: true),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    JournalId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PaymentType = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Communication = table.Column<string>(nullable: true),
                    PaymentDifferenceHandling = table.Column<string>(nullable: true),
                    WriteoffAccountId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountPayments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPayments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPayments_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPayments_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPayments_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPayments_AccountAccounts_WriteoffAccountId",
                        column: x => x.WriteoffAccountId,
                        principalTable: "AccountAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountFullReconciles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ExchangeMoveId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountFullReconciles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountFullReconciles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountFullReconciles_AccountMoves_ExchangeMoveId",
                        column: x => x.ExchangeMoveId,
                        principalTable: "AccountMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountFullReconciles_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Origin = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    RefundInvoiceId = table.Column<Guid>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    MoveName = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Sent = table.Column<bool>(nullable: false),
                    DateInvoice = table.Column<DateTime>(nullable: true),
                    DateDue = table.Column<DateTime>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    MoveId = table.Column<Guid>(nullable: true),
                    AmountUntaxed = table.Column<decimal>(nullable: false),
                    AmountTax = table.Column<decimal>(nullable: false),
                    AmountTotal = table.Column<decimal>(nullable: false),
                    JournalId = table.Column<Guid>(nullable: false),
                    Reconciled = table.Column<bool>(nullable: false),
                    Residual = table.Column<decimal>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    AmountTotalCompanySigned = table.Column<decimal>(nullable: false),
                    AmountTotalSigned = table.Column<decimal>(nullable: false),
                    ResidualSigned = table.Column<decimal>(nullable: false),
                    ResidualCompanySigned = table.Column<decimal>(nullable: false),
                    AmountUntaxedSigned = table.Column<decimal>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_AccountAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_AccountMoves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "AccountMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_AccountInvoices_RefundInvoiceId",
                        column: x => x.RefundInvoiceId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountInvoiceLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Origin = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: true),
                    UoMId = table.Column<Guid>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    AccountId = table.Column<Guid>(nullable: false),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    PriceSubTotal = table.Column<decimal>(nullable: false),
                    PriceSubTotalSigned = table.Column<decimal>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLines_AccountAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLines_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLines_AccountInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLines_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLines_UoMs_UoMId",
                        column: x => x.UoMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountInvoicePaymentRel",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvoicePaymentRel", x => new { x.PaymentId, x.InvoiceId });
                    table.ForeignKey(
                        name: "FK_AccountInvoicePaymentRel_AccountInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountInvoicePaymentRel_AccountPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "AccountPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountMoveLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: true),
                    ProductUoMId = table.Column<Guid>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    Debit = table.Column<decimal>(nullable: false),
                    Credit = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    Ref = table.Column<string>(nullable: true),
                    JournalId = table.Column<Guid>(nullable: true),
                    DateMaturity = table.Column<DateTime>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    MoveId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    AmountResidual = table.Column<decimal>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Reconciled = table.Column<bool>(nullable: false),
                    PaymentId = table.Column<Guid>(nullable: true),
                    FullReconcileId = table.Column<Guid>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountMoveLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_AccountAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_AccountFullReconciles_FullReconcileId",
                        column: x => x.FullReconcileId,
                        principalTable: "AccountFullReconciles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_AccountInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_AccountMoves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "AccountMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_AccountPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "AccountPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_UoMs_ProductUoMId",
                        column: x => x.ProductUoMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountMoveLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountInvoiceAccountMoveLineRel",
                columns: table => new
                {
                    AccountInvoiceId = table.Column<Guid>(nullable: false),
                    MoveLineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvoiceAccountMoveLineRel", x => new { x.AccountInvoiceId, x.MoveLineId });
                    table.ForeignKey(
                        name: "FK_AccountInvoiceAccountMoveLineRel_AccountInvoices_AccountInvoiceId",
                        column: x => x.AccountInvoiceId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceAccountMoveLineRel_AccountMoveLines_MoveLineId",
                        column: x => x.MoveLineId,
                        principalTable: "AccountMoveLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountPartialReconciles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    DebitMoveId = table.Column<Guid>(nullable: false),
                    CreditMoveId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    FullReconcileId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPartialReconciles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountPartialReconciles_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPartialReconciles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPartialReconciles_AccountMoveLines_CreditMoveId",
                        column: x => x.CreditMoveId,
                        principalTable: "AccountMoveLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPartialReconciles_AccountMoveLines_DebitMoveId",
                        column: x => x.DebitMoveId,
                        principalTable: "AccountMoveLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPartialReconciles_AccountFullReconciles_FullReconcileId",
                        column: x => x.FullReconcileId,
                        principalTable: "AccountFullReconciles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPartialReconciles_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountAccounts_CompanyId",
                table: "AccountAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAccounts_CreatedById",
                table: "AccountAccounts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAccounts_UserTypeId",
                table: "AccountAccounts",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAccounts_WriteById",
                table: "AccountAccounts",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAccountTypes_CreatedById",
                table: "AccountAccountTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAccountTypes_WriteById",
                table: "AccountAccountTypes",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFullReconciles_CreatedById",
                table: "AccountFullReconciles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFullReconciles_ExchangeMoveId",
                table: "AccountFullReconciles",
                column: "ExchangeMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFullReconciles_WriteById",
                table: "AccountFullReconciles",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceAccountMoveLineRel_MoveLineId",
                table: "AccountInvoiceAccountMoveLineRel",
                column: "MoveLineId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_AccountId",
                table: "AccountInvoiceLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_CompanyId",
                table: "AccountInvoiceLines",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_CreatedById",
                table: "AccountInvoiceLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_InvoiceId",
                table: "AccountInvoiceLines",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_PartnerId",
                table: "AccountInvoiceLines",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_ProductId",
                table: "AccountInvoiceLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_UoMId",
                table: "AccountInvoiceLines",
                column: "UoMId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_WriteById",
                table: "AccountInvoiceLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoicePaymentRel_InvoiceId",
                table: "AccountInvoicePaymentRel",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_AccountId",
                table: "AccountInvoices",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_CompanyId",
                table: "AccountInvoices",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_CreatedById",
                table: "AccountInvoices",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_JournalId",
                table: "AccountInvoices",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_MoveId",
                table: "AccountInvoices",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_PartnerId",
                table: "AccountInvoices",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_RefundInvoiceId",
                table: "AccountInvoices",
                column: "RefundInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_UserId",
                table: "AccountInvoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_WriteById",
                table: "AccountInvoices",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountJournals_CompanyId",
                table: "AccountJournals",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountJournals_CreatedById",
                table: "AccountJournals",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountJournals_DefaultCreditAccountId",
                table: "AccountJournals",
                column: "DefaultCreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountJournals_DefaultDebitAccountId",
                table: "AccountJournals",
                column: "DefaultDebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountJournals_RefundSequenceId",
                table: "AccountJournals",
                column: "RefundSequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountJournals_SequenceId",
                table: "AccountJournals",
                column: "SequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountJournals_WriteById",
                table: "AccountJournals",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_AccountId",
                table: "AccountMoveLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_CompanyId",
                table: "AccountMoveLines",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_CreatedById",
                table: "AccountMoveLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_FullReconcileId",
                table: "AccountMoveLines",
                column: "FullReconcileId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_InvoiceId",
                table: "AccountMoveLines",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_JournalId",
                table: "AccountMoveLines",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_MoveId",
                table: "AccountMoveLines",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_PartnerId",
                table: "AccountMoveLines",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_PaymentId",
                table: "AccountMoveLines",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_ProductId",
                table: "AccountMoveLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_ProductUoMId",
                table: "AccountMoveLines",
                column: "ProductUoMId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_WriteById",
                table: "AccountMoveLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoves_CompanyId",
                table: "AccountMoves",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoves_CreatedById",
                table: "AccountMoves",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoves_JournalId",
                table: "AccountMoves",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoves_PartnerId",
                table: "AccountMoves",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoves_WriteById",
                table: "AccountMoves",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPartialReconciles_CompanyId",
                table: "AccountPartialReconciles",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPartialReconciles_CreatedById",
                table: "AccountPartialReconciles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPartialReconciles_CreditMoveId",
                table: "AccountPartialReconciles",
                column: "CreditMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPartialReconciles_DebitMoveId",
                table: "AccountPartialReconciles",
                column: "DebitMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPartialReconciles_FullReconcileId",
                table: "AccountPartialReconciles",
                column: "FullReconcileId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPartialReconciles_WriteById",
                table: "AccountPartialReconciles",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayments_CompanyId",
                table: "AccountPayments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayments_CreatedById",
                table: "AccountPayments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayments_JournalId",
                table: "AccountPayments",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayments_PartnerId",
                table: "AccountPayments",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayments_WriteById",
                table: "AccountPayments",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayments_WriteoffAccountId",
                table: "AccountPayments",
                column: "WriteoffAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountInvoiceAccountMoveLineRel");

            migrationBuilder.DropTable(
                name: "AccountInvoiceLines");

            migrationBuilder.DropTable(
                name: "AccountInvoicePaymentRel");

            migrationBuilder.DropTable(
                name: "AccountPartialReconciles");

            migrationBuilder.DropTable(
                name: "AccountMoveLines");

            migrationBuilder.DropTable(
                name: "AccountFullReconciles");

            migrationBuilder.DropTable(
                name: "AccountInvoices");

            migrationBuilder.DropTable(
                name: "AccountPayments");

            migrationBuilder.DropTable(
                name: "AccountMoves");

            migrationBuilder.DropTable(
                name: "AccountJournals");

            migrationBuilder.DropTable(
                name: "AccountAccounts");

            migrationBuilder.DropTable(
                name: "AccountAccountTypes");
        }
    }
}
