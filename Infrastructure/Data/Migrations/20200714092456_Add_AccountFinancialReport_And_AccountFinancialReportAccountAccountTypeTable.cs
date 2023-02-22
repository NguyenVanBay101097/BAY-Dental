using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Add_AccountFinancialReport_And_AccountFinancialReportAccountAccountTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountFinancialReports",
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
                    table.PrimaryKey("PK_AccountFinancialReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountFinancialReports_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountFinancialReports_AccountFinancialReports_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AccountFinancialReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountFinancialReports_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountFinancialReportAccountAccountTypeRels",
                columns: table => new
                {
                    AccountTypeId = table.Column<Guid>(nullable: false),
                    FinancialReportId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountFinancialReportAccountAccountTypeRels", x => new { x.AccountTypeId, x.FinancialReportId });
                    table.ForeignKey(
                        name: "FK_AccountFinancialReportAccountAccountTypeRels_AccountAccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountAccountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountFinancialReportAccountAccountTypeRels_AccountFinancialReports_FinancialReportId",
                        column: x => x.FinancialReportId,
                        principalTable: "AccountFinancialReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountFinancialReportAccountAccountTypeRels_FinancialReportId",
                table: "AccountFinancialReportAccountAccountTypeRels",
                column: "FinancialReportId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFinancialReports_CreatedById",
                table: "AccountFinancialReports",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFinancialReports_ParentId",
                table: "AccountFinancialReports",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFinancialReports_WriteById",
                table: "AccountFinancialReports",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountFinancialReportAccountAccountTypeRels");

            migrationBuilder.DropTable(
                name: "AccountFinancialReports");
        }
    }
}
