using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CompanyId = table.Column<Guid>(nullable: true),
                    EmployeeId = table.Column<Guid>(nullable: true),
                    ToothCategoryId = table.Column<Guid>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Diagnostic = table.Column<string>(nullable: true),
                    ToothId = table.Column<Guid>(nullable: true),
                    LaboLineId = table.Column<Guid>(nullable: true),
                    PurchaseLineId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvoiceLines", x => x.Id);
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
                    Name = table.Column<string>(nullable: true),
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
                    AmountTotal = table.Column<decimal>(nullable: false),
                    JournalId = table.Column<Guid>(nullable: false),
                    Reconciled = table.Column<bool>(nullable: false),
                    Residual = table.Column<decimal>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    AmountTotalSigned = table.Column<decimal>(nullable: false),
                    ResidualSigned = table.Column<decimal>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    AmountTax = table.Column<decimal>(nullable: false),
                    AmountUntaxed = table.Column<decimal>(nullable: false),
                    DateOrder = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    DiscountType = table.Column<string>(nullable: true),
                    DiscountPercent = table.Column<decimal>(nullable: false),
                    DiscountFixed = table.Column<decimal>(nullable: false),
                    DiscountAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_AccountInvoices_RefundInvoiceId",
                        column: x => x.RefundInvoiceId,
                        principalTable: "AccountInvoices",
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
                    DedicatedRefund = table.Column<bool>(nullable: false),
                    BankAccountId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountJournals", x => x.Id);
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
                        name: "FK_AccountPayments_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
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
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    PeriodLockDate = table.Column<DateTime>(nullable: true),
                    AccountIncomeId = table.Column<Guid>(nullable: true),
                    AccountExpenseId = table.Column<Guid>(nullable: true),
                    Logo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
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
                });

            migrationBuilder.CreateTable(
                name: "AccountInvoiceLineToothRel",
                columns: table => new
                {
                    InvoiceLineId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvoiceLineToothRel", x => new { x.InvoiceLineId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLineToothRel_AccountInvoiceLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "AccountInvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderLineInvoiceRels",
                columns: table => new
                {
                    OrderLineId = table.Column<Guid>(nullable: false),
                    InvoiceLineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLineInvoiceRels", x => new { x.OrderLineId, x.InvoiceLineId });
                    table.ForeignKey(
                        name: "FK_SaleOrderLineInvoiceRels_AccountInvoiceLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "AccountInvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                });

            migrationBuilder.CreateTable(
                name: "DotKhams",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    DoctorId = table.Column<Guid>(nullable: true),
                    AssistantId = table.Column<Guid>(nullable: true),
                    AppointmentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DotKhams_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhams_AccountInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DotKhamSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    DotKhamId = table.Column<Guid>(nullable: true),
                    InvoicesId = table.Column<Guid>(nullable: true),
                    SaleLineId = table.Column<Guid>(nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    IsDone = table.Column<bool>(nullable: false),
                    Order = table.Column<int>(nullable: true),
                    IsInclude = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhamSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DotKhamSteps_DotKhams_DotKhamId",
                        column: x => x.DotKhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhamSteps_AccountInvoices_InvoicesId",
                        column: x => x.InvoicesId,
                        principalTable: "AccountInvoices",
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
                        name: "FK_AccountMoves_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                        name: "FK_AccountRegisterPayments_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
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
                        name: "FK_AccountFullReconciles_AccountMoves_ExchangeMoveId",
                        column: x => x.ExchangeMoveId,
                        principalTable: "AccountMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentMailMessageRels",
                columns: table => new
                {
                    AppointmentId = table.Column<Guid>(nullable: false),
                    MailMessageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentMailMessageRels", x => new { x.AppointmentId, x.MailMessageId });
                });

            migrationBuilder.CreateTable(
                name: "ApplicationRoleFunctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Func = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRoleFunctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationRoleFunctions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    DotKhamId = table.Column<Guid>(nullable: true),
                    DoctorId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_DotKhams_DotKhamId",
                        column: x => x.DotKhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "CardCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    ActivatedDate = table.Column<DateTime>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    TotalPoint = table.Column<decimal>(nullable: true),
                    PointInPeriod = table.Column<decimal>(nullable: true),
                    ExpiredDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CardId = table.Column<Guid>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    PointInPeriod = table.Column<decimal>(nullable: true),
                    TotalPoint = table.Column<decimal>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardHistories_CardCards_CardId",
                        column: x => x.CardId,
                        principalTable: "CardCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    BasicPoint = table.Column<decimal>(nullable: true),
                    Discount = table.Column<decimal>(nullable: false),
                    PricelistId = table.Column<Guid>(nullable: true),
                    NbPeriod = table.Column<int>(nullable: false),
                    Period = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DotKhamLineOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    LineId = table.Column<Guid>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateFinished = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhamLineOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DotKhamLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    DotKhamId = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    RoutingId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateFinished = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhamLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DotKhamLines_DotKhams_DotKhamId",
                        column: x => x.DotKhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Ref = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    IdentityCard = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    BirthDay = table.Column<DateTime>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    IsDoctor = table.Column<bool>(nullable: false),
                    IsAssistant = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_EmployeeCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "EmployeeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Histories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IrAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    DatasFname = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ResName = table.Column<string>(nullable: true),
                    ResField = table.Column<string>(nullable: true),
                    ResModel = table.Column<string>(nullable: true),
                    ResId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    DbDatas = table.Column<byte[]>(nullable: true),
                    MineType = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    FileSize = table.Column<int>(nullable: true),
                    UploadId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrAttachments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IrConfigParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrConfigParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IRModelAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    PermRead = table.Column<bool>(nullable: false),
                    PermWrite = table.Column<bool>(nullable: false),
                    PermCreate = table.Column<bool>(nullable: false),
                    PermUnlink = table.Column<bool>(nullable: false),
                    ModelId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRModelAccesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IRModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ResId = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: false),
                    Module = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IRModelFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Model = table.Column<string>(nullable: false),
                    IRModelId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TType = table.Column<string>(nullable: false),
                    Relation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRModelFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IRModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Model = table.Column<string>(nullable: false),
                    Transient = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IrModuleCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    Visible = table.Column<bool>(nullable: true),
                    Exclusive = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrModuleCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrModuleCategories_IrModuleCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "IrModuleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IRProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ValueFloat = table.Column<double>(nullable: true),
                    ValueInteger = table.Column<int>(nullable: true),
                    ValueText = table.Column<string>(nullable: true),
                    ValueBinary = table.Column<byte[]>(nullable: true),
                    ValueReference = table.Column<string>(nullable: true),
                    ValueDateTime = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    FieldId = table.Column<Guid>(nullable: false),
                    ResId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRProperties_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRProperties_IRModelFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "IRModelFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IRRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Global = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    PermUnlink = table.Column<bool>(nullable: false),
                    PermWrite = table.Column<bool>(nullable: false),
                    PermRead = table.Column<bool>(nullable: false),
                    PermCreate = table.Column<bool>(nullable: false),
                    ModelId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRRules_IRModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "IRModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IRSequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    NumberNext = table.Column<int>(nullable: false),
                    Implementation = table.Column<string>(nullable: true),
                    Padding = table.Column<int>(nullable: false),
                    NumberIncrement = table.Column<int>(nullable: false),
                    Prefix = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Suffix = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRSequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRSequences_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    ProductQty = table.Column<decimal>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    PriceSubtotal = table.Column<decimal>(nullable: false),
                    PriceTotal = table.Column<decimal>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    WarrantyCode = table.Column<string>(nullable: true),
                    WarrantyPeriod = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    ToothCategoryId = table.Column<Guid>(nullable: true),
                    QtyInvoiced = table.Column<decimal>(nullable: false),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboOrderLines_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    PartnerRef = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: true),
                    DateOrder = table.Column<DateTime>(nullable: false),
                    AmountTotal = table.Column<decimal>(nullable: false),
                    DatePlanned = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    DotKhamId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrders_DotKhams_DotKhamId",
                        column: x => x.DotKhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MailMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    ResId = table.Column<Guid>(nullable: true),
                    RecordName = table.Column<string>(nullable: true),
                    MessageType = table.Column<string>(nullable: false),
                    EmailFrom = table.Column<string>(nullable: true),
                    AuthorId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    MailMessageId = table.Column<Guid>(nullable: false),
                    ResPartnerId = table.Column<Guid>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailNotifications_MailMessages_MailMessageId",
                        column: x => x.MailMessageId,
                        principalTable: "MailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MailTrackingValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Field = table.Column<string>(nullable: false),
                    FieldDesc = table.Column<string>(nullable: false),
                    FieldType = table.Column<string>(nullable: true),
                    OldValueInteger = table.Column<int>(nullable: true),
                    OldValueDicimal = table.Column<decimal>(nullable: true),
                    OldValueText = table.Column<string>(nullable: true),
                    OldValueDateTime = table.Column<DateTime>(nullable: true),
                    NewValueInteger = table.Column<int>(nullable: true),
                    NewValueDecimal = table.Column<decimal>(nullable: true),
                    NewValueText = table.Column<string>(nullable: true),
                    NewValueDateTime = table.Column<DateTime>(nullable: true),
                    MailMessageId = table.Column<Guid>(nullable: false),
                    TrackSequence = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTrackingValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailTrackingValues_MailMessages_MailMessageId",
                        column: x => x.MailMessageId,
                        principalTable: "MailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartnerCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    CompleteName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    ParentLeft = table.Column<int>(nullable: true),
                    ParentRight = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerCategories_PartnerCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PartnerCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    NameNoSign = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Supplier = table.Column<bool>(nullable: false),
                    Customer = table.Column<bool>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Ref = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Employee = table.Column<bool>(nullable: false),
                    Gender = table.Column<string>(nullable: true),
                    JobTitle = table.Column<string>(nullable: true),
                    BirthYear = table.Column<int>(nullable: true),
                    BirthMonth = table.Column<int>(nullable: true),
                    BirthDay = table.Column<int>(nullable: true),
                    MedicalHistory = table.Column<string>(nullable: true),
                    CityCode = table.Column<string>(nullable: true),
                    CityName = table.Column<string>(nullable: true),
                    DistrictCode = table.Column<string>(nullable: true),
                    DistrictName = table.Column<string>(nullable: true),
                    WardCode = table.Column<string>(nullable: true),
                    WardName = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<Guid>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    ZaloId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partners_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partners_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    IsUserRoot = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MailMessageResPartnerRels",
                columns: table => new
                {
                    MailMessageId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailMessageResPartnerRels", x => new { x.MailMessageId, x.PartnerId });
                    table.ForeignKey(
                        name: "FK_MailMessageResPartnerRels_MailMessages_MailMessageId",
                        column: x => x.MailMessageId,
                        principalTable: "MailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailMessageResPartnerRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartnerHistoryRels",
                columns: table => new
                {
                    PartnerId = table.Column<Guid>(nullable: false),
                    HistoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerHistoryRels", x => new { x.PartnerId, x.HistoryId });
                    table.ForeignKey(
                        name: "FK_PartnerHistoryRels_Histories_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "Histories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerHistoryRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartnerPartnerCategoryRel",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerPartnerCategoryRel", x => new { x.CategoryId, x.PartnerId });
                    table.ForeignKey(
                        name: "FK_PartnerPartnerCategoryRel_PartnerCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "PartnerCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerPartnerCategoryRel_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CompleteName = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    ParentLeft = table.Column<int>(nullable: true),
                    ParentRight = table.Column<int>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    ServiceCateg = table.Column<bool>(nullable: false),
                    MedicineCateg = table.Column<bool>(nullable: false),
                    LaboCateg = table.Column<bool>(nullable: false),
                    ProductCateg = table.Column<bool>(nullable: false),
                    StepCateg = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductCategories_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductPricelists",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateEnd = table.Column<DateTime>(nullable: true),
                    PartnerCategId = table.Column<Guid>(nullable: true),
                    CardTypeId = table.Column<Guid>(nullable: true),
                    DiscountPolicy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPricelists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPricelists_CardTypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "CardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelists_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelists_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelists_PartnerCategories_PartnerCategId",
                        column: x => x.PartnerCategId,
                        principalTable: "PartnerCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelists_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    MaximumUseNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionPrograms_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionPrograms_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResBanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    BIC = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResBanks_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResBanks_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResCompanyUsersRels",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResCompanyUsersRels", x => new { x.CompanyId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ResCompanyUsersRels_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResCompanyUsersRels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResConfigSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    GroupDiscountPerSOLine = table.Column<bool>(nullable: true),
                    GroupSaleCouponPromotion = table.Column<bool>(nullable: true),
                    GroupLoyaltyCard = table.Column<bool>(nullable: true),
                    LoyaltyPointExchangeRate = table.Column<decimal>(nullable: true),
                    GroupMultiCompany = table.Column<bool>(nullable: true),
                    CompanyShareProduct = table.Column<bool>(nullable: true),
                    CompanySharePartner = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResConfigSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResConfigSettings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResConfigSettings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResConfigSettings_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResGroups_IrModuleCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "IrModuleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ResGroups_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResGroups_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PointExchangeRate = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleSettings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleSettings_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Usage = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ParentLocationId = table.Column<Guid>(nullable: true),
                    CompleteName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    ScrapLocation = table.Column<bool>(nullable: false),
                    ParentLeft = table.Column<int>(nullable: true),
                    ParentRight = table.Column<int>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    NameGet = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockLocations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLocations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLocations_StockLocations_ParentLocationId",
                        column: x => x.ParentLocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLocations_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToaThuocs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    DotKhamId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaThuocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_DotKhams_DotKhamId",
                        column: x => x.DotKhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToothCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Sequence = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToothCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToothCategories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToothCategories_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UoMCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    MeasureType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UoMCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UoMCategories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UoMCategories_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Token = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Expiration = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRefreshTokens_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRefreshTokens_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZaloOAConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    AccessToken = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    AutoSendBirthdayMessage = table.Column<bool>(nullable: false),
                    BirthdayMessageContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZaloOAConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZaloOAConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZaloOAConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionProgramCompanyRels",
                columns: table => new
                {
                    PromotionProgramId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionProgramCompanyRels", x => new { x.PromotionProgramId, x.CompanyId });
                    table.ForeignKey(
                        name: "FK_PromotionProgramCompanyRels_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionProgramCompanyRels_PromotionPrograms_PromotionProgramId",
                        column: x => x.PromotionProgramId,
                        principalTable: "PromotionPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResPartnerBanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    AccountNumber = table.Column<string>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false),
                    BankId = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResPartnerBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResPartnerBanks_ResBanks_BankId",
                        column: x => x.BankId,
                        principalTable: "ResBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResPartnerBanks_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResPartnerBanks_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResPartnerBanks_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResPartnerBanks_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResGroupImpliedRels",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    HId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResGroupImpliedRels", x => new { x.GId, x.HId });
                    table.ForeignKey(
                        name: "FK_ResGroupImpliedRels_ResGroups_GId",
                        column: x => x.GId,
                        principalTable: "ResGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResGroupImpliedRels_ResGroups_HId",
                        column: x => x.HId,
                        principalTable: "ResGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResGroupsUsersRels",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResGroupsUsersRels", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ResGroupsUsersRels_ResGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ResGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResGroupsUsersRels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleGroupRels",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleGroupRels", x => new { x.RuleId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_RuleGroupRels_ResGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ResGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleGroupRels_IRRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "IRRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teeth",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    ViTriHam = table.Column<string>(nullable: true),
                    Position = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teeth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teeth_ToothCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ToothCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teeth_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teeth_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UoMs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Rounding = table.Column<decimal>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Factor = table.Column<decimal>(nullable: false),
                    UOMType = table.Column<string>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    MeasureType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UoMs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UoMs_UoMCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "UoMCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UoMs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UoMs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboOrderLineToothRels",
                columns: table => new
                {
                    LaboLineId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboOrderLineToothRels", x => new { x.LaboLineId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_LaboOrderLineToothRels_LaboOrderLines_LaboLineId",
                        column: x => x.LaboLineId,
                        principalTable: "LaboOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboOrderLineToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    NameNoSign = table.Column<string>(nullable: true),
                    UOMId = table.Column<Guid>(nullable: false),
                    CategId = table.Column<Guid>(nullable: false),
                    ListPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaleOK = table.Column<bool>(nullable: false),
                    KeToaOK = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    UOMPOId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    PurchaseOK = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    KeToaNote = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    DefaultCode = table.Column<string>(nullable: true),
                    NameGet = table.Column<string>(nullable: true),
                    IsLabo = table.Column<bool>(nullable: false),
                    Type2 = table.Column<string>(nullable: true),
                    PurchasePrice = table.Column<decimal>(nullable: true),
                    LaboPrice = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_CategId",
                        column: x => x.CategId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_UoMs_UOMId",
                        column: x => x.UOMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_UoMs_UOMPOId",
                        column: x => x.UOMPOId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductCompanyRels",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    StandardPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCompanyRels", x => new { x.ProductId, x.CompanyId });
                    table.ForeignKey(
                        name: "FK_ProductCompanyRels_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCompanyRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPriceHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: true),
                    Cost = table.Column<double>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPriceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPriceHistories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPriceHistories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPriceHistories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPriceHistories_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductPricelistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    CategId = table.Column<Guid>(nullable: true),
                    AppliedOn = table.Column<string>(nullable: true),
                    MinQuantity = table.Column<int>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    Base = table.Column<string>(nullable: true),
                    PriceListId = table.Column<Guid>(nullable: true),
                    PriceSurcharge = table.Column<decimal>(nullable: true),
                    PriceDiscount = table.Column<decimal>(nullable: true),
                    PriceRound = table.Column<decimal>(nullable: true),
                    PriceMinMargin = table.Column<decimal>(nullable: true),
                    PriceMaxMargin = table.Column<decimal>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateEnd = table.Column<DateTime>(nullable: true),
                    ComputePrice = table.Column<string>(nullable: true),
                    FixedPrice = table.Column<decimal>(nullable: true),
                    PercentPrice = table.Column<decimal>(nullable: true),
                    FixedAmountPrice = table.Column<decimal>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    PartnerCategId = table.Column<Guid>(nullable: true),
                    CardTypeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPricelistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_CardTypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "CardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_ProductCategories_CategId",
                        column: x => x.CategId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_PartnerCategories_PartnerCategId",
                        column: x => x.PartnerCategId,
                        principalTable: "PartnerCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_ProductPricelists_PriceListId",
                        column: x => x.PriceListId,
                        principalTable: "ProductPricelists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPricelistItems_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Default = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSteps_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSteps_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSteps_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ProgramId = table.Column<Guid>(nullable: false),
                    MinQuantity = table.Column<decimal>(nullable: true),
                    DiscountType = table.Column<string>(nullable: true),
                    DiscountPercentage = table.Column<decimal>(nullable: true),
                    DiscountFixedAmount = table.Column<decimal>(nullable: true),
                    DiscountApplyOn = table.Column<string>(nullable: true),
                    DiscountLineProductId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionRules_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionRules_Products_DiscountLineProductId",
                        column: x => x.DiscountLineProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionRules_PromotionPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "PromotionPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionRules_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Routings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routings_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleCouponPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Sequence = table.Column<int>(nullable: true),
                    MaximumUseNumber = table.Column<int>(nullable: true),
                    RuleMinimumAmount = table.Column<decimal>(nullable: true),
                    RuleMinQuantity = table.Column<int>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    DiscountType = table.Column<string>(nullable: true),
                    DiscountPercentage = table.Column<decimal>(nullable: true),
                    DiscountFixedAmount = table.Column<decimal>(nullable: true),
                    DiscountLineProductId = table.Column<Guid>(nullable: true),
                    ValidityDuration = table.Column<int>(nullable: true),
                    RewardType = table.Column<string>(nullable: true),
                    ProgramType = table.Column<string>(nullable: true),
                    PromoCodeUsage = table.Column<string>(nullable: true),
                    PromoCode = table.Column<string>(nullable: true),
                    PromoApplicability = table.Column<string>(nullable: true),
                    DiscountMaxAmount = table.Column<decimal>(nullable: true),
                    RewardProductId = table.Column<Guid>(nullable: true),
                    RewardProductQuantity = table.Column<int>(nullable: true),
                    RuleDateFrom = table.Column<DateTime>(nullable: true),
                    RuleDateTo = table.Column<DateTime>(nullable: true),
                    RewardDescription = table.Column<string>(nullable: true),
                    DiscountApplyOn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleCouponPrograms_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleCouponPrograms_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleCouponPrograms_Products_DiscountLineProductId",
                        column: x => x.DiscountLineProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleCouponPrograms_Products_RewardProductId",
                        column: x => x.RewardProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleCouponPrograms_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToaThuocLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ToaThuocId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: true),
                    NumberOfTimes = table.Column<int>(nullable: false),
                    NumberOfDays = table.Column<int>(nullable: false),
                    AmountOfTimes = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    UseAt = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaThuocLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToaThuocLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocLines_ToaThuocs_ToaThuocId",
                        column: x => x.ToaThuocId,
                        principalTable: "ToaThuocs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToaThuocLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRuleProductCategoryRels",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(nullable: false),
                    CategId = table.Column<Guid>(nullable: false),
                    DiscountLineProductId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRuleProductCategoryRels", x => new { x.RuleId, x.CategId });
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductCategoryRels_ProductCategories_CategId",
                        column: x => x.CategId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductCategoryRels_Products_DiscountLineProductId",
                        column: x => x.DiscountLineProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductCategoryRels_PromotionRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "PromotionRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRuleProductRels",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    DiscountLineProductId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRuleProductRels", x => new { x.RuleId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductRels_Products_DiscountLineProductId",
                        column: x => x.DiscountLineProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductRels_PromotionRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "PromotionRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutingLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    RoutingId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutingLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutingLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutingLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutingLines_Routings_RoutingId",
                        column: x => x.RoutingId,
                        principalTable: "Routings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutingLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleCouponProgramProductRels",
                columns: table => new
                {
                    ProgramId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponProgramProductRels", x => new { x.ProgramId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramProductRels_SaleCouponPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    DateOrder = table.Column<DateTime>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false),
                    AmountTax = table.Column<decimal>(nullable: true),
                    AmountUntaxed = table.Column<decimal>(nullable: true),
                    AmountTotal = table.Column<decimal>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    InvoiceStatus = table.Column<string>(nullable: true),
                    Residual = table.Column<decimal>(nullable: true),
                    CardId = table.Column<Guid>(nullable: true),
                    PricelistId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    IsQuotation = table.Column<bool>(nullable: true),
                    QuoteId = table.Column<Guid>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: true),
                    CodePromoProgramId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrders_CardCards_CardId",
                        column: x => x.CardId,
                        principalTable: "CardCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrders_SaleCouponPrograms_CodePromoProgramId",
                        column: x => x.CodePromoProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SaleOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrders_SaleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrders_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrders_ProductPricelists_PricelistId",
                        column: x => x.PricelistId,
                        principalTable: "ProductPricelists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrders_SaleOrders_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrders_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleCoupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    ProgramId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    DateExpired = table.Column<DateTime>(nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleCoupons_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleCoupons_SaleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleCoupons_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleCoupons_SaleCouponPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SaleCoupons_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleCoupons_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderNoCodePromoPrograms",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(nullable: false),
                    ProgramId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderNoCodePromoPrograms", x => new { x.OrderId, x.ProgramId });
                    table.ForeignKey(
                        name: "FK_SaleOrderNoCodePromoPrograms_SaleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderNoCodePromoPrograms_SaleCouponPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    ProductUOMQty = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    OrderPartnerId = table.Column<Guid>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: false),
                    ProductUOMId = table.Column<Guid>(nullable: true),
                    Discount = table.Column<decimal>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    PriceSubTotal = table.Column<decimal>(nullable: false),
                    PriceTax = table.Column<decimal>(nullable: false),
                    PriceTotal = table.Column<decimal>(nullable: false),
                    SalesmanId = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    InvoiceStatus = table.Column<string>(nullable: true),
                    QtyToInvoice = table.Column<decimal>(nullable: true),
                    QtyInvoiced = table.Column<decimal>(nullable: true),
                    AmountToInvoice = table.Column<decimal>(nullable: true),
                    AmountInvoiced = table.Column<decimal>(nullable: true),
                    ToothCategoryId = table.Column<Guid>(nullable: true),
                    Diagnostic = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    PromotionProgramId = table.Column<Guid>(nullable: true),
                    PromotionId = table.Column<Guid>(nullable: true),
                    CouponId = table.Column<Guid>(nullable: true),
                    IsRewardLine = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_SaleCoupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "SaleCoupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_SaleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_Partners_OrderPartnerId",
                        column: x => x.OrderPartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_UoMs_ProductUOMId",
                        column: x => x.ProductUOMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_PromotionPrograms_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "PromotionPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_SaleCouponPrograms_PromotionProgramId",
                        column: x => x.PromotionProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_AspNetUsers_SalesmanId",
                        column: x => x.SalesmanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_ToothCategories_ToothCategoryId",
                        column: x => x.ToothCategoryId,
                        principalTable: "ToothCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderLineToothRels",
                columns: table => new
                {
                    SaleLineId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLineToothRels", x => new { x.SaleLineId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_SaleOrderLineToothRels_SaleOrderLines_SaleLineId",
                        column: x => x.SaleLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLineToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    ProductQty = table.Column<decimal>(nullable: false),
                    ProductUOMQty = table.Column<decimal>(nullable: true),
                    ProductUOMId = table.Column<Guid>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    PriceSubtotal = table.Column<decimal>(nullable: true),
                    PriceTotal = table.Column<decimal>(nullable: true),
                    PriceTax = table.Column<decimal>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    DatePlanned = table.Column<DateTime>(nullable: true),
                    Discount = table.Column<decimal>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    QtyInvoiced = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_UoMs_ProductUOMId",
                        column: x => x.ProductUOMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    PartnerRef = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    DateOrder = table.Column<DateTime>(nullable: false),
                    DateApprove = table.Column<DateTime>(nullable: true),
                    PickingTypeId = table.Column<Guid>(nullable: false),
                    AmountTotal = table.Column<decimal>(nullable: true),
                    AmountUntaxed = table.Column<decimal>(nullable: true),
                    AmountTax = table.Column<decimal>(nullable: true),
                    Origin = table.Column<string>(nullable: true),
                    DatePlanned = table.Column<DateTime>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    InvoiceStatus = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    RefundOrderId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_PurchaseOrders_RefundOrderId",
                        column: x => x.RefundOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockMoves",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    DateExpected = table.Column<DateTime>(nullable: true),
                    PickingTypeId = table.Column<Guid>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    ProductUOMQty = table.Column<decimal>(nullable: false),
                    ProductQty = table.Column<decimal>(nullable: true),
                    ProductUOMId = table.Column<Guid>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    LocationDestId = table.Column<Guid>(nullable: false),
                    WarehouseId = table.Column<Guid>(nullable: true),
                    PickingId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    PriceUnit = table.Column<double>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    Origin = table.Column<string>(nullable: true),
                    PurchaseLineId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMoves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMoves_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_StockLocations_LocationDestId",
                        column: x => x.LocationDestId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_UoMs_ProductUOMId",
                        column: x => x.ProductUOMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_PurchaseOrderLines_PurchaseLineId",
                        column: x => x.PurchaseLineId,
                        principalTable: "PurchaseOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockQuants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    Cost = table.Column<double>(nullable: true),
                    LocationId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    InDate = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    NegativeMoveId = table.Column<Guid>(nullable: true),
                    PropagatedFromId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockQuants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockQuants_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_StockMoves_NegativeMoveId",
                        column: x => x.NegativeMoveId,
                        principalTable: "StockMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_StockQuants_PropagatedFromId",
                        column: x => x.PropagatedFromId,
                        principalTable: "StockQuants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockQuantMoveRel",
                columns: table => new
                {
                    QuantId = table.Column<Guid>(nullable: false),
                    MoveId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockQuantMoveRel", x => new { x.MoveId, x.QuantId });
                    table.ForeignKey(
                        name: "FK_StockQuantMoveRel_StockMoves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "StockMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockQuantMoveRel_StockQuants_QuantId",
                        column: x => x.QuantId,
                        principalTable: "StockQuants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockPickings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    PickingTypeId = table.Column<Guid>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    LocationDestId = table.Column<Guid>(nullable: false),
                    Origin = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPickings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockPickings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_StockLocations_LocationDestId",
                        column: x => x.LocationDestId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockPickingTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    DefaultLocationDestId = table.Column<Guid>(nullable: true),
                    WarehouseId = table.Column<Guid>(nullable: true),
                    IRSequenceId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DefaultLocationSrcId = table.Column<Guid>(nullable: true),
                    ReturnPickingTypeId = table.Column<Guid>(nullable: true),
                    UseCreateLots = table.Column<bool>(nullable: true),
                    UseExistingLots = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPickingTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_StockLocations_DefaultLocationDestId",
                        column: x => x.DefaultLocationDestId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_StockLocations_DefaultLocationSrcId",
                        column: x => x.DefaultLocationSrcId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_IRSequences_IRSequenceId",
                        column: x => x.IRSequenceId,
                        principalTable: "IRSequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_StockPickingTypes_ReturnPickingTypeId",
                        column: x => x.ReturnPickingTypeId,
                        principalTable: "StockPickingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockWarehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: true),
                    ViewLocationId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    InTypeId = table.Column<Guid>(nullable: true),
                    OutTypeId = table.Column<Guid>(nullable: true),
                    ReceptionSteps = table.Column<string>(nullable: true),
                    DeliverySteps = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockWarehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_StockPickingTypes_InTypeId",
                        column: x => x.InTypeId,
                        principalTable: "StockPickingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_StockPickingTypes_OutTypeId",
                        column: x => x.OutTypeId,
                        principalTable: "StockPickingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_StockLocations_ViewLocationId",
                        column: x => x.ViewLocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_AspNetUsers_WriteById",
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
                name: "IX_AccountInvoiceLines_EmployeeId",
                table: "AccountInvoiceLines",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_InvoiceId",
                table: "AccountInvoiceLines",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_LaboLineId",
                table: "AccountInvoiceLines",
                column: "LaboLineId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_PartnerId",
                table: "AccountInvoiceLines",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_ProductId",
                table: "AccountInvoiceLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_PurchaseLineId",
                table: "AccountInvoiceLines",
                column: "PurchaseLineId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_ToothCategoryId",
                table: "AccountInvoiceLines",
                column: "ToothCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_ToothId",
                table: "AccountInvoiceLines",
                column: "ToothId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_UoMId",
                table: "AccountInvoiceLines",
                column: "UoMId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_WriteById",
                table: "AccountInvoiceLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLineToothRel_ToothId",
                table: "AccountInvoiceLineToothRel",
                column: "ToothId");

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
                name: "IX_AccountJournals_BankAccountId",
                table: "AccountJournals",
                column: "BankAccountId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRoleFunctions_CreatedById",
                table: "ApplicationRoleFunctions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRoleFunctions_RoleId",
                table: "ApplicationRoleFunctions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRoleFunctions_WriteById",
                table: "ApplicationRoleFunctions",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMailMessageRels_MailMessageId",
                table: "AppointmentMailMessageRels",
                column: "MailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CompanyId",
                table: "Appointments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CreatedById",
                table: "Appointments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DotKhamId",
                table: "Appointments",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PartnerId",
                table: "Appointments",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserId",
                table: "Appointments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_WriteById",
                table: "Appointments",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PartnerId",
                table: "AspNetUsers",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CardCards_CreatedById",
                table: "CardCards",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CardCards_PartnerId",
                table: "CardCards",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CardCards_TypeId",
                table: "CardCards",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CardCards_WriteById",
                table: "CardCards",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_CardHistories_CardId",
                table: "CardHistories",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardHistories_CreatedById",
                table: "CardHistories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CardHistories_TypeId",
                table: "CardHistories",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CardHistories_WriteById",
                table: "CardHistories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_CardTypes_CreatedById",
                table: "CardTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CardTypes_PricelistId",
                table: "CardTypes",
                column: "PricelistId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTypes_WriteById",
                table: "CardTypes",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AccountExpenseId",
                table: "Companies",
                column: "AccountExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AccountIncomeId",
                table: "Companies",
                column: "AccountIncomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CreatedById",
                table: "Companies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PartnerId",
                table: "Companies",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_WriteById",
                table: "Companies",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLineOperations_CreatedById",
                table: "DotKhamLineOperations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLineOperations_LineId",
                table: "DotKhamLineOperations",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLineOperations_ProductId",
                table: "DotKhamLineOperations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLineOperations_WriteById",
                table: "DotKhamLineOperations",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_CreatedById",
                table: "DotKhamLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_DotKhamId",
                table: "DotKhamLines",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_ProductId",
                table: "DotKhamLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_RoutingId",
                table: "DotKhamLines",
                column: "RoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_UserId",
                table: "DotKhamLines",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_WriteById",
                table: "DotKhamLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AppointmentId",
                table: "DotKhams",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_CompanyId",
                table: "DotKhams",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_CreatedById",
                table: "DotKhams",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_DoctorId",
                table: "DotKhams",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_InvoiceId",
                table: "DotKhams",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_PartnerId",
                table: "DotKhams",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_SaleOrderId",
                table: "DotKhams",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_UserId",
                table: "DotKhams",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_WriteById",
                table: "DotKhams",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_CreatedById",
                table: "DotKhamSteps",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_DotKhamId",
                table: "DotKhamSteps",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_InvoicesId",
                table: "DotKhamSteps",
                column: "InvoicesId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_ProductId",
                table: "DotKhamSteps",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_SaleLineId",
                table: "DotKhamSteps",
                column: "SaleLineId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_SaleOrderId",
                table: "DotKhamSteps",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_WriteById",
                table: "DotKhamSteps",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCategories_CreatedById",
                table: "EmployeeCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCategories_WriteById",
                table: "EmployeeCategories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CategoryId",
                table: "Employees",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CreatedById",
                table: "Employees",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_WriteById",
                table: "Employees",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_CreatedById",
                table: "Histories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_WriteById",
                table: "Histories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IrAttachments_CompanyId",
                table: "IrAttachments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IrAttachments_CreatedById",
                table: "IrAttachments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IrAttachments_WriteById",
                table: "IrAttachments",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IrConfigParameters_CreatedById",
                table: "IrConfigParameters",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IrConfigParameters_WriteById",
                table: "IrConfigParameters",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelAccesses_CreatedById",
                table: "IRModelAccesses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelAccesses_GroupId",
                table: "IRModelAccesses",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelAccesses_ModelId",
                table: "IRModelAccesses",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelAccesses_WriteById",
                table: "IRModelAccesses",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelDatas_CreatedById",
                table: "IRModelDatas",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelDatas_WriteById",
                table: "IRModelDatas",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelFields_CreatedById",
                table: "IRModelFields",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelFields_IRModelId",
                table: "IRModelFields",
                column: "IRModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelFields_WriteById",
                table: "IRModelFields",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModels_CreatedById",
                table: "IRModels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModels_WriteById",
                table: "IRModels",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IrModuleCategories_CreatedById",
                table: "IrModuleCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IrModuleCategories_ParentId",
                table: "IrModuleCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_IrModuleCategories_WriteById",
                table: "IrModuleCategories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IRProperties_CompanyId",
                table: "IRProperties",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IRProperties_CreatedById",
                table: "IRProperties",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRProperties_FieldId",
                table: "IRProperties",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_IRProperties_WriteById",
                table: "IRProperties",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IRRules_CreatedById",
                table: "IRRules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRRules_ModelId",
                table: "IRRules",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IRRules_WriteById",
                table: "IRRules",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IRSequences_CompanyId",
                table: "IRSequences",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IRSequences_CreatedById",
                table: "IRSequences",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRSequences_WriteById",
                table: "IRSequences",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_CompanyId",
                table: "LaboOrderLines",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_CreatedById",
                table: "LaboOrderLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_CustomerId",
                table: "LaboOrderLines",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_OrderId",
                table: "LaboOrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_PartnerId",
                table: "LaboOrderLines",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_ProductId",
                table: "LaboOrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_ToothCategoryId",
                table: "LaboOrderLines",
                column: "ToothCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_WriteById",
                table: "LaboOrderLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLineToothRels_ToothId",
                table: "LaboOrderLineToothRels",
                column: "ToothId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_CompanyId",
                table: "LaboOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_CreatedById",
                table: "LaboOrders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_CustomerId",
                table: "LaboOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_DotKhamId",
                table: "LaboOrders",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_PartnerId",
                table: "LaboOrders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_SaleOrderId",
                table: "LaboOrders",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_UserId",
                table: "LaboOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_WriteById",
                table: "LaboOrders",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessageResPartnerRels_PartnerId",
                table: "MailMessageResPartnerRels",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_AuthorId",
                table: "MailMessages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_CreatedById",
                table: "MailMessages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_WriteById",
                table: "MailMessages",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_MailNotifications_CreatedById",
                table: "MailNotifications",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MailNotifications_MailMessageId",
                table: "MailNotifications",
                column: "MailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MailNotifications_ResPartnerId",
                table: "MailNotifications",
                column: "ResPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MailNotifications_WriteById",
                table: "MailNotifications",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_MailTrackingValues_CreatedById",
                table: "MailTrackingValues",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MailTrackingValues_MailMessageId",
                table: "MailTrackingValues",
                column: "MailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MailTrackingValues_WriteById",
                table: "MailTrackingValues",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerCategories_CreatedById",
                table: "PartnerCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerCategories_ParentId",
                table: "PartnerCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerCategories_WriteById",
                table: "PartnerCategories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerHistoryRels_HistoryId",
                table: "PartnerHistoryRels",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPartnerCategoryRel_PartnerId",
                table: "PartnerPartnerCategoryRel",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_CompanyId",
                table: "Partners",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_CreatedById",
                table: "Partners",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_EmployeeId",
                table: "Partners",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_WriteById",
                table: "Partners",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CreatedById",
                table: "ProductCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentId",
                table: "ProductCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_WriteById",
                table: "ProductCategories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCompanyRels_CompanyId",
                table: "ProductCompanyRels",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceHistories_CompanyId",
                table: "ProductPriceHistories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceHistories_CreatedById",
                table: "ProductPriceHistories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceHistories_ProductId",
                table: "ProductPriceHistories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceHistories_WriteById",
                table: "ProductPriceHistories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_CardTypeId",
                table: "ProductPricelistItems",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_CategId",
                table: "ProductPricelistItems",
                column: "CategId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_CompanyId",
                table: "ProductPricelistItems",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_CreatedById",
                table: "ProductPricelistItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_PartnerCategId",
                table: "ProductPricelistItems",
                column: "PartnerCategId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_PriceListId",
                table: "ProductPricelistItems",
                column: "PriceListId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_ProductId",
                table: "ProductPricelistItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_WriteById",
                table: "ProductPricelistItems",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_CardTypeId",
                table: "ProductPricelists",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_CompanyId",
                table: "ProductPricelists",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_CreatedById",
                table: "ProductPricelists",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_PartnerCategId",
                table: "ProductPricelists",
                column: "PartnerCategId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_WriteById",
                table: "ProductPricelists",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategId",
                table: "Products",
                column: "CategId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CompanyId",
                table: "Products",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedById",
                table: "Products",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UOMId",
                table: "Products",
                column: "UOMId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UOMPOId",
                table: "Products",
                column: "UOMPOId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_WriteById",
                table: "Products",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSteps_CreatedById",
                table: "ProductSteps",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSteps_ProductId",
                table: "ProductSteps",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSteps_WriteById",
                table: "ProductSteps",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionProgramCompanyRels_CompanyId",
                table: "PromotionProgramCompanyRels",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionPrograms_CreatedById",
                table: "PromotionPrograms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionPrograms_WriteById",
                table: "PromotionPrograms",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleProductCategoryRels_CategId",
                table: "PromotionRuleProductCategoryRels",
                column: "CategId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleProductCategoryRels_DiscountLineProductId",
                table: "PromotionRuleProductCategoryRels",
                column: "DiscountLineProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleProductRels_DiscountLineProductId",
                table: "PromotionRuleProductRels",
                column: "DiscountLineProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleProductRels_ProductId",
                table: "PromotionRuleProductRels",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRules_CreatedById",
                table: "PromotionRules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRules_DiscountLineProductId",
                table: "PromotionRules",
                column: "DiscountLineProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRules_ProgramId",
                table: "PromotionRules",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRules_WriteById",
                table: "PromotionRules",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_CompanyId",
                table: "PurchaseOrderLines",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_CreatedById",
                table: "PurchaseOrderLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_OrderId",
                table: "PurchaseOrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PartnerId",
                table: "PurchaseOrderLines",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_ProductId",
                table: "PurchaseOrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_ProductUOMId",
                table: "PurchaseOrderLines",
                column: "ProductUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_WriteById",
                table: "PurchaseOrderLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CompanyId",
                table: "PurchaseOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CreatedById",
                table: "PurchaseOrders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PartnerId",
                table: "PurchaseOrders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PickingTypeId",
                table: "PurchaseOrders",
                column: "PickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_RefundOrderId",
                table: "PurchaseOrders",
                column: "RefundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_UserId",
                table: "PurchaseOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_WriteById",
                table: "PurchaseOrders",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResBanks_CreatedById",
                table: "ResBanks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResBanks_WriteById",
                table: "ResBanks",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResCompanyUsersRels_UserId",
                table: "ResCompanyUsersRels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResConfigSettings_CompanyId",
                table: "ResConfigSettings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResConfigSettings_CreatedById",
                table: "ResConfigSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResConfigSettings_WriteById",
                table: "ResConfigSettings",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResGroupImpliedRels_HId",
                table: "ResGroupImpliedRels",
                column: "HId");

            migrationBuilder.CreateIndex(
                name: "IX_ResGroups_CategoryId",
                table: "ResGroups",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ResGroups_CreatedById",
                table: "ResGroups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResGroups_WriteById",
                table: "ResGroups",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResGroupsUsersRels_UserId",
                table: "ResGroupsUsersRels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResPartnerBanks_BankId",
                table: "ResPartnerBanks",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_ResPartnerBanks_CompanyId",
                table: "ResPartnerBanks",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResPartnerBanks_CreatedById",
                table: "ResPartnerBanks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResPartnerBanks_PartnerId",
                table: "ResPartnerBanks",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ResPartnerBanks_WriteById",
                table: "ResPartnerBanks",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingLines_CreatedById",
                table: "RoutingLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingLines_ProductId",
                table: "RoutingLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingLines_RoutingId",
                table: "RoutingLines",
                column: "RoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingLines_WriteById",
                table: "RoutingLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Routings_CreatedById",
                table: "Routings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Routings_ProductId",
                table: "Routings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Routings_WriteById",
                table: "Routings",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_RuleGroupRels_GroupId",
                table: "RuleGroupRels",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponProgramProductRels_ProductId",
                table: "SaleCouponProgramProductRels",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponPrograms_CompanyId",
                table: "SaleCouponPrograms",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponPrograms_CreatedById",
                table: "SaleCouponPrograms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponPrograms_DiscountLineProductId",
                table: "SaleCouponPrograms",
                column: "DiscountLineProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponPrograms_RewardProductId",
                table: "SaleCouponPrograms",
                column: "RewardProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponPrograms_WriteById",
                table: "SaleCouponPrograms",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCoupons_CreatedById",
                table: "SaleCoupons",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCoupons_OrderId",
                table: "SaleCoupons",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCoupons_PartnerId",
                table: "SaleCoupons",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCoupons_ProgramId",
                table: "SaleCoupons",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCoupons_SaleOrderId",
                table: "SaleCoupons",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCoupons_WriteById",
                table: "SaleCoupons",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineInvoiceRels_InvoiceLineId",
                table: "SaleOrderLineInvoiceRels",
                column: "InvoiceLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_CompanyId",
                table: "SaleOrderLines",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_CouponId",
                table: "SaleOrderLines",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_CreatedById",
                table: "SaleOrderLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_OrderId",
                table: "SaleOrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_OrderPartnerId",
                table: "SaleOrderLines",
                column: "OrderPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_ProductId",
                table: "SaleOrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_ProductUOMId",
                table: "SaleOrderLines",
                column: "ProductUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_PromotionId",
                table: "SaleOrderLines",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_PromotionProgramId",
                table: "SaleOrderLines",
                column: "PromotionProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_SalesmanId",
                table: "SaleOrderLines",
                column: "SalesmanId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_ToothCategoryId",
                table: "SaleOrderLines",
                column: "ToothCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_WriteById",
                table: "SaleOrderLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineToothRels_ToothId",
                table: "SaleOrderLineToothRels",
                column: "ToothId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderNoCodePromoPrograms_ProgramId",
                table: "SaleOrderNoCodePromoPrograms",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_CardId",
                table: "SaleOrders",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_CodePromoProgramId",
                table: "SaleOrders",
                column: "CodePromoProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_CompanyId",
                table: "SaleOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_CreatedById",
                table: "SaleOrders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_OrderId",
                table: "SaleOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_PartnerId",
                table: "SaleOrders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_PricelistId",
                table: "SaleOrders",
                column: "PricelistId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_QuoteId",
                table: "SaleOrders",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_UserId",
                table: "SaleOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_WriteById",
                table: "SaleOrders",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleSettings_CreatedById",
                table: "SaleSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleSettings_WriteById",
                table: "SaleSettings",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockLocations_CompanyId",
                table: "StockLocations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLocations_CreatedById",
                table: "StockLocations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockLocations_ParentLocationId",
                table: "StockLocations",
                column: "ParentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLocations_WriteById",
                table: "StockLocations",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_CompanyId",
                table: "StockMoves",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_CreatedById",
                table: "StockMoves",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_LocationDestId",
                table: "StockMoves",
                column: "LocationDestId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_LocationId",
                table: "StockMoves",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_PartnerId",
                table: "StockMoves",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_PickingId",
                table: "StockMoves",
                column: "PickingId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_PickingTypeId",
                table: "StockMoves",
                column: "PickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_ProductId",
                table: "StockMoves",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_ProductUOMId",
                table: "StockMoves",
                column: "ProductUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_PurchaseLineId",
                table: "StockMoves",
                column: "PurchaseLineId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_WarehouseId",
                table: "StockMoves",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_WriteById",
                table: "StockMoves",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_CompanyId",
                table: "StockPickings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_CreatedById",
                table: "StockPickings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_LocationDestId",
                table: "StockPickings",
                column: "LocationDestId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_LocationId",
                table: "StockPickings",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_PartnerId",
                table: "StockPickings",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_PickingTypeId",
                table: "StockPickings",
                column: "PickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_WriteById",
                table: "StockPickings",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_CreatedById",
                table: "StockPickingTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_DefaultLocationDestId",
                table: "StockPickingTypes",
                column: "DefaultLocationDestId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_DefaultLocationSrcId",
                table: "StockPickingTypes",
                column: "DefaultLocationSrcId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_IRSequenceId",
                table: "StockPickingTypes",
                column: "IRSequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_ReturnPickingTypeId",
                table: "StockPickingTypes",
                column: "ReturnPickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_WarehouseId",
                table: "StockPickingTypes",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_WriteById",
                table: "StockPickingTypes",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuantMoveRel_QuantId",
                table: "StockQuantMoveRel",
                column: "QuantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_CompanyId",
                table: "StockQuants",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_CreatedById",
                table: "StockQuants",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_LocationId",
                table: "StockQuants",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_NegativeMoveId",
                table: "StockQuants",
                column: "NegativeMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_ProductId",
                table: "StockQuants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_PropagatedFromId",
                table: "StockQuants",
                column: "PropagatedFromId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_WriteById",
                table: "StockQuants",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_CompanyId",
                table: "StockWarehouses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_CreatedById",
                table: "StockWarehouses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_InTypeId",
                table: "StockWarehouses",
                column: "InTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_LocationId",
                table: "StockWarehouses",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_OutTypeId",
                table: "StockWarehouses",
                column: "OutTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_PartnerId",
                table: "StockWarehouses",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_ViewLocationId",
                table: "StockWarehouses",
                column: "ViewLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_WriteById",
                table: "StockWarehouses",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Teeth_CategoryId",
                table: "Teeth",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Teeth_CreatedById",
                table: "Teeth",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Teeth_WriteById",
                table: "Teeth",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocLines_CreatedById",
                table: "ToaThuocLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocLines_ProductId",
                table: "ToaThuocLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocLines_ToaThuocId",
                table: "ToaThuocLines",
                column: "ToaThuocId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocLines_WriteById",
                table: "ToaThuocLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_CompanyId",
                table: "ToaThuocs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_CreatedById",
                table: "ToaThuocs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_DotKhamId",
                table: "ToaThuocs",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_PartnerId",
                table: "ToaThuocs",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_UserId",
                table: "ToaThuocs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_WriteById",
                table: "ToaThuocs",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ToothCategories_CreatedById",
                table: "ToothCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToothCategories_WriteById",
                table: "ToothCategories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_UoMCategories_CreatedById",
                table: "UoMCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UoMCategories_WriteById",
                table: "UoMCategories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_UoMs_CategoryId",
                table: "UoMs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UoMs_CreatedById",
                table: "UoMs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UoMs_WriteById",
                table: "UoMs",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_CreatedById",
                table: "UserRefreshTokens",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_UserId",
                table: "UserRefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_WriteById",
                table: "UserRefreshTokens",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ZaloOAConfigs_CreatedById",
                table: "ZaloOAConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ZaloOAConfigs_WriteById",
                table: "ZaloOAConfigs",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_Companies_CompanyId",
                table: "AccountInvoiceLines",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_AspNetUsers_CreatedById",
                table: "AccountInvoiceLines",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_AspNetUsers_WriteById",
                table: "AccountInvoiceLines",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_AccountInvoices_InvoiceId",
                table: "AccountInvoiceLines",
                column: "InvoiceId",
                principalTable: "AccountInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_AccountAccounts_AccountId",
                table: "AccountInvoiceLines",
                column: "AccountId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_Partners_EmployeeId",
                table: "AccountInvoiceLines",
                column: "EmployeeId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_Partners_PartnerId",
                table: "AccountInvoiceLines",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_LaboOrderLines_LaboLineId",
                table: "AccountInvoiceLines",
                column: "LaboLineId",
                principalTable: "LaboOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_Products_ProductId",
                table: "AccountInvoiceLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_PurchaseOrderLines_PurchaseLineId",
                table: "AccountInvoiceLines",
                column: "PurchaseLineId",
                principalTable: "PurchaseOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_ToothCategories_ToothCategoryId",
                table: "AccountInvoiceLines",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_Teeth_ToothId",
                table: "AccountInvoiceLines",
                column: "ToothId",
                principalTable: "Teeth",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_UoMs_UoMId",
                table: "AccountInvoiceLines",
                column: "UoMId",
                principalTable: "UoMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoices_Companies_CompanyId",
                table: "AccountInvoices",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoices_AspNetUsers_CreatedById",
                table: "AccountInvoices",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoices_AspNetUsers_UserId",
                table: "AccountInvoices",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoices_AspNetUsers_WriteById",
                table: "AccountInvoices",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoices_AccountMoves_MoveId",
                table: "AccountInvoices",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoices_AccountAccounts_AccountId",
                table: "AccountInvoices",
                column: "AccountId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoices_Partners_PartnerId",
                table: "AccountInvoices",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoices_AccountJournals_JournalId",
                table: "AccountInvoices",
                column: "JournalId",
                principalTable: "AccountJournals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_Companies_CompanyId",
                table: "AccountJournals",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_AspNetUsers_CreatedById",
                table: "AccountJournals",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_AspNetUsers_WriteById",
                table: "AccountJournals",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_AccountAccounts_DefaultCreditAccountId",
                table: "AccountJournals",
                column: "DefaultCreditAccountId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_AccountAccounts_DefaultDebitAccountId",
                table: "AccountJournals",
                column: "DefaultDebitAccountId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_ResPartnerBanks_BankAccountId",
                table: "AccountJournals",
                column: "BankAccountId",
                principalTable: "ResPartnerBanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_IRSequences_RefundSequenceId",
                table: "AccountJournals",
                column: "RefundSequenceId",
                principalTable: "IRSequences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_IRSequences_SequenceId",
                table: "AccountJournals",
                column: "SequenceId",
                principalTable: "IRSequences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_Companies_CompanyId",
                table: "AccountMoveLines",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AspNetUsers_CreatedById",
                table: "AccountMoveLines",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AspNetUsers_WriteById",
                table: "AccountMoveLines",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountMoves_MoveId",
                table: "AccountMoveLines",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountAccounts_AccountId",
                table: "AccountMoveLines",
                column: "AccountId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_Partners_PartnerId",
                table: "AccountMoveLines",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_Products_ProductId",
                table: "AccountMoveLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_UoMs_ProductUoMId",
                table: "AccountMoveLines",
                column: "ProductUoMId",
                principalTable: "UoMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountPayments_PaymentId",
                table: "AccountMoveLines",
                column: "PaymentId",
                principalTable: "AccountPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountFullReconciles_FullReconcileId",
                table: "AccountMoveLines",
                column: "FullReconcileId",
                principalTable: "AccountFullReconciles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPayments_Companies_CompanyId",
                table: "AccountPayments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPayments_AspNetUsers_CreatedById",
                table: "AccountPayments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPayments_AspNetUsers_WriteById",
                table: "AccountPayments",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPayments_AccountAccounts_WriteoffAccountId",
                table: "AccountPayments",
                column: "WriteoffAccountId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPayments_Partners_PartnerId",
                table: "AccountPayments",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_CreatedById",
                table: "Companies",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_WriteById",
                table: "Companies",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AccountAccounts_AccountExpenseId",
                table: "Companies",
                column: "AccountExpenseId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AccountAccounts_AccountIncomeId",
                table: "Companies",
                column: "AccountIncomeId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Partners_PartnerId",
                table: "Companies",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountAccounts_AspNetUsers_CreatedById",
                table: "AccountAccounts",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountAccounts_AspNetUsers_WriteById",
                table: "AccountAccounts",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountAccounts_AccountAccountTypes_UserTypeId",
                table: "AccountAccounts",
                column: "UserTypeId",
                principalTable: "AccountAccountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPartialReconciles_AspNetUsers_CreatedById",
                table: "AccountPartialReconciles",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPartialReconciles_AspNetUsers_WriteById",
                table: "AccountPartialReconciles",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPartialReconciles_AccountFullReconciles_FullReconcileId",
                table: "AccountPartialReconciles",
                column: "FullReconcileId",
                principalTable: "AccountFullReconciles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLineToothRel_Teeth_ToothId",
                table: "AccountInvoiceLineToothRel",
                column: "ToothId",
                principalTable: "Teeth",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLineInvoiceRels_SaleOrderLines_OrderLineId",
                table: "SaleOrderLineInvoiceRels",
                column: "OrderLineId",
                principalTable: "SaleOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRegisterPaymentInvoiceRel_AccountRegisterPayments_PaymentId",
                table: "AccountRegisterPaymentInvoiceRel",
                column: "PaymentId",
                principalTable: "AccountRegisterPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_AspNetUsers_CreatedById",
                table: "DotKhams",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_AspNetUsers_UserId",
                table: "DotKhams",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_AspNetUsers_WriteById",
                table: "DotKhams",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Partners_PartnerId",
                table: "DotKhams",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Appointments_AppointmentId",
                table: "DotKhams",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams",
                column: "AssistantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_DoctorId",
                table: "DotKhams",
                column: "DoctorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_SaleOrders_SaleOrderId",
                table: "DotKhams",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_AspNetUsers_CreatedById",
                table: "DotKhamSteps",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_AspNetUsers_WriteById",
                table: "DotKhamSteps",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_Products_ProductId",
                table: "DotKhamSteps",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_SaleOrders_SaleOrderId",
                table: "DotKhamSteps",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_SaleOrderLines_SaleLineId",
                table: "DotKhamSteps",
                column: "SaleLineId",
                principalTable: "SaleOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoves_AspNetUsers_CreatedById",
                table: "AccountMoves",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoves_AspNetUsers_WriteById",
                table: "AccountMoves",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoves_Partners_PartnerId",
                table: "AccountMoves",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRegisterPayments_AspNetUsers_CreatedById",
                table: "AccountRegisterPayments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRegisterPayments_AspNetUsers_WriteById",
                table: "AccountRegisterPayments",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRegisterPayments_Partners_PartnerId",
                table: "AccountRegisterPayments",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountFullReconciles_AspNetUsers_CreatedById",
                table: "AccountFullReconciles",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountFullReconciles_AspNetUsers_WriteById",
                table: "AccountFullReconciles",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentMailMessageRels_Appointments_AppointmentId",
                table: "AppointmentMailMessageRels",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentMailMessageRels_MailMessages_MailMessageId",
                table: "AppointmentMailMessageRels",
                column: "MailMessageId",
                principalTable: "MailMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationRoleFunctions_AspNetUsers_CreatedById",
                table: "ApplicationRoleFunctions",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationRoleFunctions_AspNetUsers_WriteById",
                table: "ApplicationRoleFunctions",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountAccountTypes_AspNetUsers_CreatedById",
                table: "AccountAccountTypes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountAccountTypes_AspNetUsers_WriteById",
                table: "AccountAccountTypes",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_CreatedById",
                table: "Appointments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_UserId",
                table: "Appointments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_WriteById",
                table: "Appointments",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Partners_PartnerId",
                table: "Appointments",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Employees_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CardCards_AspNetUsers_CreatedById",
                table: "CardCards",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardCards_AspNetUsers_WriteById",
                table: "CardCards",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardCards_Partners_PartnerId",
                table: "CardCards",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardCards_CardTypes_TypeId",
                table: "CardCards",
                column: "TypeId",
                principalTable: "CardTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardHistories_AspNetUsers_CreatedById",
                table: "CardHistories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardHistories_AspNetUsers_WriteById",
                table: "CardHistories",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardHistories_CardTypes_TypeId",
                table: "CardHistories",
                column: "TypeId",
                principalTable: "CardTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardTypes_AspNetUsers_CreatedById",
                table: "CardTypes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardTypes_AspNetUsers_WriteById",
                table: "CardTypes",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardTypes_ProductPricelists_PricelistId",
                table: "CardTypes",
                column: "PricelistId",
                principalTable: "ProductPricelists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLineOperations_AspNetUsers_CreatedById",
                table: "DotKhamLineOperations",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLineOperations_AspNetUsers_WriteById",
                table: "DotKhamLineOperations",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLineOperations_Products_ProductId",
                table: "DotKhamLineOperations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLineOperations_DotKhamLines_LineId",
                table: "DotKhamLineOperations",
                column: "LineId",
                principalTable: "DotKhamLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_AspNetUsers_CreatedById",
                table: "DotKhamLines",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_AspNetUsers_UserId",
                table: "DotKhamLines",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_AspNetUsers_WriteById",
                table: "DotKhamLines",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_Products_ProductId",
                table: "DotKhamLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_Routings_RoutingId",
                table: "DotKhamLines",
                column: "RoutingId",
                principalTable: "Routings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCategories_AspNetUsers_CreatedById",
                table: "EmployeeCategories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCategories_AspNetUsers_WriteById",
                table: "EmployeeCategories",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById",
                table: "Employees",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_WriteById",
                table: "Employees",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_AspNetUsers_CreatedById",
                table: "Histories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_AspNetUsers_WriteById",
                table: "Histories",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IrAttachments_AspNetUsers_CreatedById",
                table: "IrAttachments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IrAttachments_AspNetUsers_WriteById",
                table: "IrAttachments",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IrConfigParameters_AspNetUsers_CreatedById",
                table: "IrConfigParameters",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IrConfigParameters_AspNetUsers_WriteById",
                table: "IrConfigParameters",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModelAccesses_AspNetUsers_CreatedById",
                table: "IRModelAccesses",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModelAccesses_AspNetUsers_WriteById",
                table: "IRModelAccesses",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModelAccesses_ResGroups_GroupId",
                table: "IRModelAccesses",
                column: "GroupId",
                principalTable: "ResGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModelAccesses_IRModels_ModelId",
                table: "IRModelAccesses",
                column: "ModelId",
                principalTable: "IRModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModelDatas_AspNetUsers_CreatedById",
                table: "IRModelDatas",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModelDatas_AspNetUsers_WriteById",
                table: "IRModelDatas",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModelFields_AspNetUsers_CreatedById",
                table: "IRModelFields",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModelFields_AspNetUsers_WriteById",
                table: "IRModelFields",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModelFields_IRModels_IRModelId",
                table: "IRModelFields",
                column: "IRModelId",
                principalTable: "IRModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModels_AspNetUsers_CreatedById",
                table: "IRModels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRModels_AspNetUsers_WriteById",
                table: "IRModels",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IrModuleCategories_AspNetUsers_CreatedById",
                table: "IrModuleCategories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IrModuleCategories_AspNetUsers_WriteById",
                table: "IrModuleCategories",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRProperties_AspNetUsers_CreatedById",
                table: "IRProperties",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRProperties_AspNetUsers_WriteById",
                table: "IRProperties",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRRules_AspNetUsers_CreatedById",
                table: "IRRules",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRRules_AspNetUsers_WriteById",
                table: "IRRules",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRSequences_AspNetUsers_CreatedById",
                table: "IRSequences",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IRSequences_AspNetUsers_WriteById",
                table: "IRSequences",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_AspNetUsers_CreatedById",
                table: "LaboOrderLines",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_AspNetUsers_WriteById",
                table: "LaboOrderLines",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_Partners_CustomerId",
                table: "LaboOrderLines",
                column: "CustomerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_Partners_PartnerId",
                table: "LaboOrderLines",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_Products_ProductId",
                table: "LaboOrderLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_ToothCategories_ToothCategoryId",
                table: "LaboOrderLines",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_LaboOrders_OrderId",
                table: "LaboOrderLines",
                column: "OrderId",
                principalTable: "LaboOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_AspNetUsers_CreatedById",
                table: "LaboOrders",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_AspNetUsers_UserId",
                table: "LaboOrders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_AspNetUsers_WriteById",
                table: "LaboOrders",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_Partners_CustomerId",
                table: "LaboOrders",
                column: "CustomerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_Partners_PartnerId",
                table: "LaboOrders",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_SaleOrders_SaleOrderId",
                table: "LaboOrders",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MailMessages_AspNetUsers_CreatedById",
                table: "MailMessages",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MailMessages_AspNetUsers_WriteById",
                table: "MailMessages",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MailMessages_Partners_AuthorId",
                table: "MailMessages",
                column: "AuthorId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MailNotifications_AspNetUsers_CreatedById",
                table: "MailNotifications",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MailNotifications_AspNetUsers_WriteById",
                table: "MailNotifications",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MailNotifications_Partners_ResPartnerId",
                table: "MailNotifications",
                column: "ResPartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MailTrackingValues_AspNetUsers_CreatedById",
                table: "MailTrackingValues",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MailTrackingValues_AspNetUsers_WriteById",
                table: "MailTrackingValues",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerCategories_AspNetUsers_CreatedById",
                table: "PartnerCategories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerCategories_AspNetUsers_WriteById",
                table: "PartnerCategories",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_AspNetUsers_CreatedById",
                table: "Partners",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_AspNetUsers_WriteById",
                table: "Partners",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderLines_PurchaseOrders_OrderId",
                table: "PurchaseOrderLines",
                column: "OrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_StockPickingTypes_PickingTypeId",
                table: "PurchaseOrders",
                column: "PickingTypeId",
                principalTable: "StockPickingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_StockPickingTypes_PickingTypeId",
                table: "StockMoves",
                column: "PickingTypeId",
                principalTable: "StockPickingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_StockPickings_PickingId",
                table: "StockMoves",
                column: "PickingId",
                principalTable: "StockPickings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_StockWarehouses_WarehouseId",
                table: "StockMoves",
                column: "WarehouseId",
                principalTable: "StockWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockPickings_StockPickingTypes_PickingTypeId",
                table: "StockPickings",
                column: "PickingTypeId",
                principalTable: "StockPickingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockPickingTypes_StockWarehouses_WarehouseId",
                table: "StockPickingTypes",
                column: "WarehouseId",
                principalTable: "StockWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountAccounts_Companies_CompanyId",
                table: "AccountAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoices_Companies_CompanyId",
                table: "AccountInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountJournals_Companies_CompanyId",
                table: "AccountJournals");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoves_Companies_CompanyId",
                table: "AccountMoves");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Companies_CompanyId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Companies_CompanyId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_IRSequences_Companies_CompanyId",
                table: "IRSequences");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Companies_CompanyId",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPricelists_Companies_CompanyId",
                table: "ProductPricelists");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Companies_CompanyId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ResPartnerBanks_Companies_CompanyId",
                table: "ResPartnerBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleCouponPrograms_Companies_CompanyId",
                table: "SaleCouponPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_Companies_CompanyId",
                table: "SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLocations_Companies_CompanyId",
                table: "StockLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_Companies_CompanyId",
                table: "StockWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountAccounts_AspNetUsers_CreatedById",
                table: "AccountAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountAccounts_AspNetUsers_WriteById",
                table: "AccountAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountAccountTypes_AspNetUsers_CreatedById",
                table: "AccountAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountAccountTypes_AspNetUsers_WriteById",
                table: "AccountAccountTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoices_AspNetUsers_CreatedById",
                table: "AccountInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoices_AspNetUsers_UserId",
                table: "AccountInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoices_AspNetUsers_WriteById",
                table: "AccountInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountJournals_AspNetUsers_CreatedById",
                table: "AccountJournals");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountJournals_AspNetUsers_WriteById",
                table: "AccountJournals");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoves_AspNetUsers_CreatedById",
                table: "AccountMoves");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoves_AspNetUsers_WriteById",
                table: "AccountMoves");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_CreatedById",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_UserId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_WriteById",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_CardCards_AspNetUsers_CreatedById",
                table: "CardCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CardCards_AspNetUsers_WriteById",
                table: "CardCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CardTypes_AspNetUsers_CreatedById",
                table: "CardTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_CardTypes_AspNetUsers_WriteById",
                table: "CardTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AspNetUsers_CreatedById",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AspNetUsers_UserId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AspNetUsers_WriteById",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCategories_AspNetUsers_CreatedById",
                table: "EmployeeCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCategories_AspNetUsers_WriteById",
                table: "EmployeeCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_WriteById",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_IRSequences_AspNetUsers_CreatedById",
                table: "IRSequences");

            migrationBuilder.DropForeignKey(
                name: "FK_IRSequences_AspNetUsers_WriteById",
                table: "IRSequences");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerCategories_AspNetUsers_CreatedById",
                table: "PartnerCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerCategories_AspNetUsers_WriteById",
                table: "PartnerCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_AspNetUsers_CreatedById",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_AspNetUsers_WriteById",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_AspNetUsers_CreatedById",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_AspNetUsers_WriteById",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPricelists_AspNetUsers_CreatedById",
                table: "ProductPricelists");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPricelists_AspNetUsers_WriteById",
                table: "ProductPricelists");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_CreatedById",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_WriteById",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ResBanks_AspNetUsers_CreatedById",
                table: "ResBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_ResBanks_AspNetUsers_WriteById",
                table: "ResBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_ResPartnerBanks_AspNetUsers_CreatedById",
                table: "ResPartnerBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_ResPartnerBanks_AspNetUsers_WriteById",
                table: "ResPartnerBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleCouponPrograms_AspNetUsers_CreatedById",
                table: "SaleCouponPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleCouponPrograms_AspNetUsers_WriteById",
                table: "SaleCouponPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_AspNetUsers_CreatedById",
                table: "SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_AspNetUsers_UserId",
                table: "SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_AspNetUsers_WriteById",
                table: "SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLocations_AspNetUsers_CreatedById",
                table: "StockLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLocations_AspNetUsers_WriteById",
                table: "StockLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_StockPickingTypes_AspNetUsers_CreatedById",
                table: "StockPickingTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_StockPickingTypes_AspNetUsers_WriteById",
                table: "StockPickingTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_AspNetUsers_CreatedById",
                table: "StockWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_AspNetUsers_WriteById",
                table: "StockWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_UoMCategories_AspNetUsers_CreatedById",
                table: "UoMCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_UoMCategories_AspNetUsers_WriteById",
                table: "UoMCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_UoMs_AspNetUsers_CreatedById",
                table: "UoMs");

            migrationBuilder.DropForeignKey(
                name: "FK_UoMs_AspNetUsers_WriteById",
                table: "UoMs");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountAccounts_AccountAccountTypes_UserTypeId",
                table: "AccountAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoices_AccountMoves_MoveId",
                table: "AccountInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AccountInvoices_InvoiceId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Partners_PartnerId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_CardCards_Partners_PartnerId",
                table: "CardCards");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Partners_PartnerId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_Partners_PartnerId",
                table: "SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_Partners_PartnerId",
                table: "StockWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleCouponPrograms_Products_DiscountLineProductId",
                table: "SaleCouponPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleCouponPrograms_Products_RewardProductId",
                table: "SaleCouponPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_StockPickingTypes_IRSequences_IRSequenceId",
                table: "StockPickingTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Appointments_AppointmentId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPricelists_CardTypes_CardTypeId",
                table: "ProductPricelists");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_StockPickingTypes_InTypeId",
                table: "StockWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_StockPickingTypes_OutTypeId",
                table: "StockWarehouses");

            migrationBuilder.DropTable(
                name: "AccountInvoiceAccountMoveLineRel");

            migrationBuilder.DropTable(
                name: "AccountInvoiceLineToothRel");

            migrationBuilder.DropTable(
                name: "AccountInvoicePaymentRel");

            migrationBuilder.DropTable(
                name: "AccountPartialReconciles");

            migrationBuilder.DropTable(
                name: "AccountRegisterPaymentInvoiceRel");

            migrationBuilder.DropTable(
                name: "ApplicationRoleFunctions");

            migrationBuilder.DropTable(
                name: "AppointmentMailMessageRels");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CardHistories");

            migrationBuilder.DropTable(
                name: "DotKhamLineOperations");

            migrationBuilder.DropTable(
                name: "DotKhamSteps");

            migrationBuilder.DropTable(
                name: "IrAttachments");

            migrationBuilder.DropTable(
                name: "IrConfigParameters");

            migrationBuilder.DropTable(
                name: "IRModelAccesses");

            migrationBuilder.DropTable(
                name: "IRModelDatas");

            migrationBuilder.DropTable(
                name: "IRProperties");

            migrationBuilder.DropTable(
                name: "LaboOrderLineToothRels");

            migrationBuilder.DropTable(
                name: "MailMessageResPartnerRels");

            migrationBuilder.DropTable(
                name: "MailNotifications");

            migrationBuilder.DropTable(
                name: "MailTrackingValues");

            migrationBuilder.DropTable(
                name: "PartnerHistoryRels");

            migrationBuilder.DropTable(
                name: "PartnerPartnerCategoryRel");

            migrationBuilder.DropTable(
                name: "ProductCompanyRels");

            migrationBuilder.DropTable(
                name: "ProductPriceHistories");

            migrationBuilder.DropTable(
                name: "ProductPricelistItems");

            migrationBuilder.DropTable(
                name: "ProductSteps");

            migrationBuilder.DropTable(
                name: "PromotionProgramCompanyRels");

            migrationBuilder.DropTable(
                name: "PromotionRuleProductCategoryRels");

            migrationBuilder.DropTable(
                name: "PromotionRuleProductRels");

            migrationBuilder.DropTable(
                name: "ResCompanyUsersRels");

            migrationBuilder.DropTable(
                name: "ResConfigSettings");

            migrationBuilder.DropTable(
                name: "ResGroupImpliedRels");

            migrationBuilder.DropTable(
                name: "ResGroupsUsersRels");

            migrationBuilder.DropTable(
                name: "RoutingLines");

            migrationBuilder.DropTable(
                name: "RuleGroupRels");

            migrationBuilder.DropTable(
                name: "SaleCouponProgramProductRels");

            migrationBuilder.DropTable(
                name: "SaleOrderLineInvoiceRels");

            migrationBuilder.DropTable(
                name: "SaleOrderLineToothRels");

            migrationBuilder.DropTable(
                name: "SaleOrderNoCodePromoPrograms");

            migrationBuilder.DropTable(
                name: "SaleSettings");

            migrationBuilder.DropTable(
                name: "StockQuantMoveRel");

            migrationBuilder.DropTable(
                name: "ToaThuocLines");

            migrationBuilder.DropTable(
                name: "UserRefreshTokens");

            migrationBuilder.DropTable(
                name: "ZaloOAConfigs");

            migrationBuilder.DropTable(
                name: "AccountMoveLines");

            migrationBuilder.DropTable(
                name: "AccountRegisterPayments");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DotKhamLines");

            migrationBuilder.DropTable(
                name: "IRModelFields");

            migrationBuilder.DropTable(
                name: "MailMessages");

            migrationBuilder.DropTable(
                name: "Histories");

            migrationBuilder.DropTable(
                name: "PromotionRules");

            migrationBuilder.DropTable(
                name: "ResGroups");

            migrationBuilder.DropTable(
                name: "IRRules");

            migrationBuilder.DropTable(
                name: "AccountInvoiceLines");

            migrationBuilder.DropTable(
                name: "SaleOrderLines");

            migrationBuilder.DropTable(
                name: "StockQuants");

            migrationBuilder.DropTable(
                name: "ToaThuocs");

            migrationBuilder.DropTable(
                name: "AccountFullReconciles");

            migrationBuilder.DropTable(
                name: "AccountPayments");

            migrationBuilder.DropTable(
                name: "Routings");

            migrationBuilder.DropTable(
                name: "IrModuleCategories");

            migrationBuilder.DropTable(
                name: "IRModels");

            migrationBuilder.DropTable(
                name: "LaboOrderLines");

            migrationBuilder.DropTable(
                name: "Teeth");

            migrationBuilder.DropTable(
                name: "SaleCoupons");

            migrationBuilder.DropTable(
                name: "PromotionPrograms");

            migrationBuilder.DropTable(
                name: "StockMoves");

            migrationBuilder.DropTable(
                name: "LaboOrders");

            migrationBuilder.DropTable(
                name: "ToothCategories");

            migrationBuilder.DropTable(
                name: "StockPickings");

            migrationBuilder.DropTable(
                name: "PurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AccountAccountTypes");

            migrationBuilder.DropTable(
                name: "AccountMoves");

            migrationBuilder.DropTable(
                name: "AccountInvoices");

            migrationBuilder.DropTable(
                name: "AccountJournals");

            migrationBuilder.DropTable(
                name: "ResPartnerBanks");

            migrationBuilder.DropTable(
                name: "AccountAccounts");

            migrationBuilder.DropTable(
                name: "ResBanks");

            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "UoMs");

            migrationBuilder.DropTable(
                name: "UoMCategories");

            migrationBuilder.DropTable(
                name: "IRSequences");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "DotKhams");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "SaleOrders");

            migrationBuilder.DropTable(
                name: "EmployeeCategories");

            migrationBuilder.DropTable(
                name: "CardCards");

            migrationBuilder.DropTable(
                name: "SaleCouponPrograms");

            migrationBuilder.DropTable(
                name: "CardTypes");

            migrationBuilder.DropTable(
                name: "ProductPricelists");

            migrationBuilder.DropTable(
                name: "PartnerCategories");

            migrationBuilder.DropTable(
                name: "StockPickingTypes");

            migrationBuilder.DropTable(
                name: "StockWarehouses");

            migrationBuilder.DropTable(
                name: "StockLocations");
        }
    }
}
