using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class add_accountfinancialrevenuereport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountFinancialRevenueReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    Level = table.Column<int>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Sign = table.Column<int>(nullable: false),
                    DisplayDetail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountFinancialRevenueReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountFinancialRevenueReports_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountFinancialRevenueReports_AccountFinancialRevenueReports_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AccountFinancialRevenueReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountFinancialRevenueReports_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountFinancialRevenueReportAccountAccountRels",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(nullable: false),
                    FinancialReportId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountFinancialRevenueReportAccountAccountRels", x => new { x.AccountId, x.FinancialReportId });
                    table.ForeignKey(
                        name: "FK_AccountFinancialRevenueReportAccountAccountRels_AccountAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountFinancialRevenueReportAccountAccountRels_AccountFinancialRevenueReports_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountFinancialRevenueReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountFinancialRevenueReportAccountAccountTypeRels",
                columns: table => new
                {
                    AccountTypeId = table.Column<Guid>(nullable: false),
                    FinancialReportId = table.Column<Guid>(nullable: false),
                    Column = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountFinancialRevenueReportAccountAccountTypeRels", x => new { x.AccountTypeId, x.FinancialReportId });
                    table.ForeignKey(
                        name: "FK_AccountFinancialRevenueReportAccountAccountTypeRels_AccountAccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountAccountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountFinancialRevenueReportAccountAccountTypeRels_AccountFinancialRevenueReports_FinancialReportId",
                        column: x => x.FinancialReportId,
                        principalTable: "AccountFinancialRevenueReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountFinancialRevenueReportAccountAccountTypeRels_FinancialReportId",
                table: "AccountFinancialRevenueReportAccountAccountTypeRels",
                column: "FinancialReportId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFinancialRevenueReports_CreatedById",
                table: "AccountFinancialRevenueReports",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFinancialRevenueReports_ParentId",
                table: "AccountFinancialRevenueReports",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFinancialRevenueReports_WriteById",
                table: "AccountFinancialRevenueReports",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountFinancialRevenueReportAccountAccountRels");

            migrationBuilder.DropTable(
                name: "AccountFinancialRevenueReportAccountAccountTypeRels");

            migrationBuilder.DropTable(
                name: "AccountFinancialRevenueReports");
        }
    }
}
