using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTuVanBaoGiaTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_AccountMoves_MoveId",
                table: "AccountMoveLines");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Partners_PartnerId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_AccountPayments_PaymentId",
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
                name: "SaleOrderLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "PercentFixed",
                table: "CommissionProductRules");

            migrationBuilder.AddColumn<Guid>(
                name: "QuotationId",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaid",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdvisoryId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AmountDiscountTotal",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CounselorId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SaleOrderLines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ToothType",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplyPartnerOn",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Days",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApplyDayOfWeek",
                table: "SaleCouponPrograms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApplyMaxDiscount",
                table: "SaleCouponPrograms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApplyMinimumDiscount",
                table: "SaleCouponPrograms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "SaleCouponPrograms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotIncremental",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssistantCommissionId",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CounselorCommissionId",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommissionId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MoveLineId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Commissions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Percent",
                table: "CommissionProductRules",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Hidden",
                table: "AspNetRoles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TimeExpected",
                table: "Appointments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Advisory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: true),
                    ToothCategoryId = table.Column<Guid>(nullable: true),
                    ToothType = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advisory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advisory_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Advisory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advisory_Partners_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advisory_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advisory_ToothCategories_ToothCategoryId",
                        column: x => x.ToothCategoryId,
                        principalTable: "ToothCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advisory_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartnerAdvances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false),
                    JournalId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerAdvances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerAdvances_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerAdvances_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerAdvances_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerAdvances_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerAdvances_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrintPaperSizes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PaperFormat = table.Column<string>(nullable: true),
                    TopMargin = table.Column<int>(nullable: false),
                    BottomMargin = table.Column<int>(nullable: false),
                    LeftMargin = table.Column<int>(nullable: false),
                    RightMargin = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintPaperSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintPaperSizes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrintPaperSizes_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAppointmentRels",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(nullable: false),
                    AppoinmentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAppointmentRels", x => new { x.ProductId, x.AppoinmentId });
                    table.ForeignKey(
                        name: "FK_ProductAppointmentRels_Appointments_AppoinmentId",
                        column: x => x.AppoinmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAppointmentRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: true),
                    DateQuotation = table.Column<DateTime>(nullable: false),
                    DateApplies = table.Column<int>(nullable: false),
                    DateEndQuotation = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<decimal>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotations_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotations_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleCouponProgramPartnerRels",
                columns: table => new
                {
                    ProgramId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponProgramPartnerRels", x => new { x.ProgramId, x.PartnerId });
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramPartnerRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramPartnerRels_SaleCouponPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleCouponProgramProductCategoryRels",
                columns: table => new
                {
                    ProgramId = table.Column<Guid>(nullable: false),
                    ProductCategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponProgramProductCategoryRels", x => new { x.ProgramId, x.ProductCategoryId });
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramProductCategoryRels_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramProductCategoryRels_SaleCouponPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    MoveId = table.Column<Guid>(nullable: true),
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
                        name: "FK_SaleOrderPayments_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderPromotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    SaleOrderId = table.Column<Guid>(nullable: true),
                    SaleOrderLineId = table.Column<Guid>(nullable: true),
                    DiscountType = table.Column<string>(nullable: true),
                    DiscountPercent = table.Column<decimal>(nullable: true),
                    DiscountFixed = table.Column<decimal>(nullable: true),
                    SaleCouponProgramId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotions_SaleCouponPrograms_SaleCouponProgramId",
                        column: x => x.SaleCouponProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotions_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotions_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotions_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToothDiagnosis",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToothDiagnosis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToothDiagnosis_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToothDiagnosis_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToothDiagnosis_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvisoryProductRels",
                columns: table => new
                {
                    AdvisoryId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisoryProductRels", x => new { x.AdvisoryId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_AdvisoryProductRels_Advisory_AdvisoryId",
                        column: x => x.AdvisoryId,
                        principalTable: "Advisory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvisoryProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdvisoryToothRels",
                columns: table => new
                {
                    AdvisoryId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisoryToothRels", x => new { x.AdvisoryId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_AdvisoryToothRels_Advisory_AdvisoryId",
                        column: x => x.AdvisoryId,
                        principalTable: "Advisory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvisoryToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigPrints",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PaperSizeId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsInfoCompany = table.Column<bool>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigPrints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigPrints_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigPrints_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigPrints_PrintPaperSizes_PaperSizeId",
                        column: x => x.PaperSizeId,
                        principalTable: "PrintPaperSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigPrints_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentQuotations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    DiscountPercentType = table.Column<string>(nullable: true),
                    Payment = table.Column<decimal>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    QuotationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentQuotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentQuotations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentQuotations_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentQuotations_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    DiscountAmountPercent = table.Column<decimal>(nullable: true),
                    DiscountAmountFixed = table.Column<decimal>(nullable: true),
                    DiscountType = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    SubPrice = table.Column<decimal>(nullable: true),
                    AmountDiscountTotal = table.Column<double>(nullable: true),
                    Diagnostic = table.Column<string>(nullable: true),
                    ToothCategoryId = table.Column<Guid>(nullable: false),
                    QuotationId = table.Column<Guid>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: true),
                    AssistantId = table.Column<Guid>(nullable: true),
                    CounselorId = table.Column<Guid>(nullable: true),
                    AdvisoryId = table.Column<Guid>(nullable: true),
                    ToothType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationLines_Advisory_AdvisoryId",
                        column: x => x.AdvisoryId,
                        principalTable: "Advisory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationLines_Employees_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationLines_Employees_CounselorId",
                        column: x => x.CounselorId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationLines_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationLines_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationLines_ToothCategories_ToothCategoryId",
                        column: x => x.ToothCategoryId,
                        principalTable: "ToothCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "SaleOrderPromotionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    SaleOrderLineId = table.Column<Guid>(nullable: false),
                    PromotionId = table.Column<Guid>(nullable: false),
                    PriceUnit = table.Column<double>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderPromotionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotionLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotionLines_SaleOrderPromotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "SaleOrderPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotionLines_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderPromotionLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvisoryToothDiagnosisRels",
                columns: table => new
                {
                    AdvisoryId = table.Column<Guid>(nullable: false),
                    ToothDiagnosisId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisoryToothDiagnosisRels", x => new { x.AdvisoryId, x.ToothDiagnosisId });
                    table.ForeignKey(
                        name: "FK_AdvisoryToothDiagnosisRels_Advisory_AdvisoryId",
                        column: x => x.AdvisoryId,
                        principalTable: "Advisory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvisoryToothDiagnosisRels_ToothDiagnosis_ToothDiagnosisId",
                        column: x => x.ToothDiagnosisId,
                        principalTable: "ToothDiagnosis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToothDiagnosisProductRels",
                columns: table => new
                {
                    ToothDiagnosisId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToothDiagnosisProductRels", x => new { x.ToothDiagnosisId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ToothDiagnosisProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToothDiagnosisProductRels_ToothDiagnosis_ToothDiagnosisId",
                        column: x => x.ToothDiagnosisId,
                        principalTable: "ToothDiagnosis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationLineToothRels",
                columns: table => new
                {
                    QuotationLineId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationLineToothRels", x => new { x.QuotationLineId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_QuotationLineToothRels_QuotationLines_QuotationLineId",
                        column: x => x.QuotationLineId,
                        principalTable: "QuotationLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationLineToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationPromotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    QuotationId = table.Column<Guid>(nullable: true),
                    QuotationLineId = table.Column<Guid>(nullable: true),
                    DiscountType = table.Column<string>(nullable: true),
                    DiscountPercent = table.Column<decimal>(nullable: true),
                    DiscountFixed = table.Column<decimal>(nullable: true),
                    SaleCouponProgramId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationPromotions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationPromotions_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationPromotions_QuotationLines_QuotationLineId",
                        column: x => x.QuotationLineId,
                        principalTable: "QuotationLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationPromotions_SaleCouponPrograms_SaleCouponProgramId",
                        column: x => x.SaleCouponProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationPromotions_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationPromotionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    QuotationLineId = table.Column<Guid>(nullable: true),
                    PromotionId = table.Column<Guid>(nullable: false),
                    PriceUnit = table.Column<double>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationPromotionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationPromotionLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationPromotionLines_QuotationPromotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "QuotationPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationPromotionLines_QuotationLines_QuotationLineId",
                        column: x => x.QuotationLineId,
                        principalTable: "QuotationLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationPromotionLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_QuotationId",
                table: "SaleOrders",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_AdvisoryId",
                table: "SaleOrderLines",
                column: "AdvisoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_CounselorId",
                table: "SaleOrderLines",
                column: "CounselorId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_AssistantCommissionId",
                table: "Employees",
                column: "AssistantCommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CounselorCommissionId",
                table: "Employees",
                column: "CounselorCommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_CommissionId",
                table: "CommissionSettlements",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_MoveLineId",
                table: "CommissionSettlements",
                column: "MoveLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_ProductId",
                table: "CommissionSettlements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_CompanyId",
                table: "Advisory",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_CreatedById",
                table: "Advisory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_CustomerId",
                table: "Advisory",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_EmployeeId",
                table: "Advisory",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_ToothCategoryId",
                table: "Advisory",
                column: "ToothCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_WriteById",
                table: "Advisory",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisoryProductRels_ProductId",
                table: "AdvisoryProductRels",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisoryToothDiagnosisRels_ToothDiagnosisId",
                table: "AdvisoryToothDiagnosisRels",
                column: "ToothDiagnosisId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisoryToothRels_ToothId",
                table: "AdvisoryToothRels",
                column: "ToothId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigPrints_CompanyId",
                table: "ConfigPrints",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigPrints_CreatedById",
                table: "ConfigPrints",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigPrints_PaperSizeId",
                table: "ConfigPrints",
                column: "PaperSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigPrints_WriteById",
                table: "ConfigPrints",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdvances_CompanyId",
                table: "PartnerAdvances",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdvances_CreatedById",
                table: "PartnerAdvances",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdvances_JournalId",
                table: "PartnerAdvances",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdvances_PartnerId",
                table: "PartnerAdvances",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAdvances_WriteById",
                table: "PartnerAdvances",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotations_CreatedById",
                table: "PaymentQuotations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotations_QuotationId",
                table: "PaymentQuotations",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotations_WriteById",
                table: "PaymentQuotations",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintPaperSizes_CreatedById",
                table: "PrintPaperSizes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintPaperSizes_WriteById",
                table: "PrintPaperSizes",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAppointmentRels_AppoinmentId",
                table: "ProductAppointmentRels",
                column: "AppoinmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_AdvisoryId",
                table: "QuotationLines",
                column: "AdvisoryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_AssistantId",
                table: "QuotationLines",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_CounselorId",
                table: "QuotationLines",
                column: "CounselorId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_CreatedById",
                table: "QuotationLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_EmployeeId",
                table: "QuotationLines",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_ProductId",
                table: "QuotationLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_QuotationId",
                table: "QuotationLines",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_ToothCategoryId",
                table: "QuotationLines",
                column: "ToothCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_WriteById",
                table: "QuotationLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLineToothRels_ToothId",
                table: "QuotationLineToothRels",
                column: "ToothId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationPromotionLines_CreatedById",
                table: "QuotationPromotionLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationPromotionLines_PromotionId",
                table: "QuotationPromotionLines",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationPromotionLines_QuotationLineId",
                table: "QuotationPromotionLines",
                column: "QuotationLineId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationPromotionLines_WriteById",
                table: "QuotationPromotionLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationPromotions_CreatedById",
                table: "QuotationPromotions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationPromotions_QuotationId",
                table: "QuotationPromotions",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationPromotions_QuotationLineId",
                table: "QuotationPromotions",
                column: "QuotationLineId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationPromotions_SaleCouponProgramId",
                table: "QuotationPromotions",
                column: "SaleCouponProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationPromotions_WriteById",
                table: "QuotationPromotions",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CompanyId",
                table: "Quotations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CreatedById",
                table: "Quotations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_EmployeeId",
                table: "Quotations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_PartnerId",
                table: "Quotations",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_WriteById",
                table: "Quotations",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponProgramPartnerRels_PartnerId",
                table: "SaleCouponProgramPartnerRels",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponProgramProductCategoryRels_ProductCategoryId",
                table: "SaleCouponProgramProductCategoryRels",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPaymentAccountPaymentRels_PaymentId",
                table: "SaleOrderPaymentAccountPaymentRels",
                column: "PaymentId");

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
                name: "IX_SaleOrderPayments_WriteById",
                table: "SaleOrderPayments",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotionLines_CreatedById",
                table: "SaleOrderPromotionLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotionLines_PromotionId",
                table: "SaleOrderPromotionLines",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotionLines_SaleOrderLineId",
                table: "SaleOrderPromotionLines",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotionLines_WriteById",
                table: "SaleOrderPromotionLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_CreatedById",
                table: "SaleOrderPromotions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_SaleCouponProgramId",
                table: "SaleOrderPromotions",
                column: "SaleCouponProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_SaleOrderId",
                table: "SaleOrderPromotions",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_SaleOrderLineId",
                table: "SaleOrderPromotions",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_WriteById",
                table: "SaleOrderPromotions",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ToothDiagnosis_CompanyId",
                table: "ToothDiagnosis",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ToothDiagnosis_CreatedById",
                table: "ToothDiagnosis",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToothDiagnosis_WriteById",
                table: "ToothDiagnosis",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ToothDiagnosisProductRels_ProductId",
                table: "ToothDiagnosisProductRels",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountMoves_MoveId",
                table: "AccountMoveLines",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_Commissions_CommissionId",
                table: "CommissionSettlements",
                column: "CommissionId",
                principalTable: "Commissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_AccountMoveLines_MoveLineId",
                table: "CommissionSettlements",
                column: "MoveLineId",
                principalTable: "AccountMoveLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_Products_ProductId",
                table: "CommissionSettlements",
                column: "ProductId",
                principalTable: "Products",
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

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_Advisory_AdvisoryId",
                table: "SaleOrderLines",
                column: "AdvisoryId",
                principalTable: "Advisory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_Employees_CounselorId",
                table: "SaleOrderLines",
                column: "CounselorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_Quotations_QuotationId",
                table: "SaleOrders",
                column: "QuotationId",
                principalTable: "Quotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_AccountMoves_MoveId",
                table: "AccountMoveLines");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Commissions_CommissionId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_AccountMoveLines_MoveLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Products_ProductId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Commissions_AssistantCommissionId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Commissions_CounselorCommissionId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_Advisory_AdvisoryId",
                table: "SaleOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_Employees_CounselorId",
                table: "SaleOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_Quotations_QuotationId",
                table: "SaleOrders");

            migrationBuilder.DropTable(
                name: "AdvisoryProductRels");

            migrationBuilder.DropTable(
                name: "AdvisoryToothDiagnosisRels");

            migrationBuilder.DropTable(
                name: "AdvisoryToothRels");

            migrationBuilder.DropTable(
                name: "ConfigPrints");

            migrationBuilder.DropTable(
                name: "PartnerAdvances");

            migrationBuilder.DropTable(
                name: "PaymentQuotations");

            migrationBuilder.DropTable(
                name: "ProductAppointmentRels");

            migrationBuilder.DropTable(
                name: "QuotationLineToothRels");

            migrationBuilder.DropTable(
                name: "QuotationPromotionLines");

            migrationBuilder.DropTable(
                name: "SaleCouponProgramPartnerRels");

            migrationBuilder.DropTable(
                name: "SaleCouponProgramProductCategoryRels");

            migrationBuilder.DropTable(
                name: "SaleOrderPaymentAccountPaymentRels");

            migrationBuilder.DropTable(
                name: "SaleOrderPaymentHistoryLines");

            migrationBuilder.DropTable(
                name: "SaleOrderPaymentJournalLines");

            migrationBuilder.DropTable(
                name: "SaleOrderPromotionLines");

            migrationBuilder.DropTable(
                name: "ToothDiagnosisProductRels");

            migrationBuilder.DropTable(
                name: "PrintPaperSizes");

            migrationBuilder.DropTable(
                name: "QuotationPromotions");

            migrationBuilder.DropTable(
                name: "SaleOrderPayments");

            migrationBuilder.DropTable(
                name: "SaleOrderPromotions");

            migrationBuilder.DropTable(
                name: "ToothDiagnosis");

            migrationBuilder.DropTable(
                name: "QuotationLines");

            migrationBuilder.DropTable(
                name: "Advisory");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_QuotationId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_AdvisoryId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_CounselorId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_Employees_AssistantCommissionId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CounselorCommissionId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_CommissionId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_MoveLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_ProductId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "QuotationId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "TotalPaid",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "AdvisoryId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "AmountDiscountTotal",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "CounselorId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "ToothType",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "ApplyPartnerOn",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "Days",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "IsApplyDayOfWeek",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "IsApplyMaxDiscount",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "IsApplyMinimumDiscount",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "NotIncremental",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "AssistantCommissionId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CounselorCommissionId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "CommissionId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "MoveLineId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Commissions");

            migrationBuilder.DropColumn(
                name: "Percent",
                table: "CommissionProductRules");

            migrationBuilder.DropColumn(
                name: "Hidden",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "TimeExpected",
                table: "Appointments");

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
                name: "SaleOrderLineId",
                table: "CommissionSettlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentFixed",
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
                name: "IX_CommissionSettlements_SaleOrderLineId",
                table: "CommissionSettlements",
                column: "SaleOrderLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountMoves_MoveId",
                table: "AccountMoveLines",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
