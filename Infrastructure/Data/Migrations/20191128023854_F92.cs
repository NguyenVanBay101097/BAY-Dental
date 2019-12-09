using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F92 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BankAccountId",
                table: "AccountJournals",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_AccountJournals_BankAccountId",
                table: "AccountJournals",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ResBanks_CreatedById",
                table: "ResBanks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResBanks_WriteById",
                table: "ResBanks",
                column: "WriteById");

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

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_ResPartnerBanks_BankAccountId",
                table: "AccountJournals",
                column: "BankAccountId",
                principalTable: "ResPartnerBanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountJournals_ResPartnerBanks_BankAccountId",
                table: "AccountJournals");

            migrationBuilder.DropTable(
                name: "ResPartnerBanks");

            migrationBuilder.DropTable(
                name: "ResBanks");

            migrationBuilder.DropIndex(
                name: "IX_AccountJournals_BankAccountId",
                table: "AccountJournals");

            migrationBuilder.DropColumn(
                name: "BankAccountId",
                table: "AccountJournals");
        }
    }
}
