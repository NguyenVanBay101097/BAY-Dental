using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F92 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CardId",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CardTypeId",
                table: "ProductPricelists",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CardTypeId",
                table: "ProductPricelistItems",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FixedAmountPrice",
                table: "ProductPricelistItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankAccountId",
                table: "AccountJournals",
                nullable: true);

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
                    BasicPoint = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardTypes_AspNetUsers_WriteById",
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
                    Barcode = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    TotalPoint = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardCards_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardCards_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardCards_CardTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "CardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardCards_AspNetUsers_WriteById",
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
                name: "IX_SaleOrders_CardId",
                table: "SaleOrders",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelists_CardTypeId",
                table: "ProductPricelists",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricelistItems_CardTypeId",
                table: "ProductPricelistItems",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountJournals_BankAccountId",
                table: "AccountJournals",
                column: "BankAccountId");

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
                name: "IX_CardTypes_CreatedById",
                table: "CardTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CardTypes_WriteById",
                table: "CardTypes",
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
                name: "IX_SaleSettings_CreatedById",
                table: "SaleSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleSettings_WriteById",
                table: "SaleSettings",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountJournals_ResPartnerBanks_BankAccountId",
                table: "AccountJournals",
                column: "BankAccountId",
                principalTable: "ResPartnerBanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPricelistItems_CardTypes_CardTypeId",
                table: "ProductPricelistItems",
                column: "CardTypeId",
                principalTable: "CardTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPricelists_CardTypes_CardTypeId",
                table: "ProductPricelists",
                column: "CardTypeId",
                principalTable: "CardTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_CardCards_CardId",
                table: "SaleOrders",
                column: "CardId",
                principalTable: "CardCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountJournals_ResPartnerBanks_BankAccountId",
                table: "AccountJournals");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPricelistItems_CardTypes_CardTypeId",
                table: "ProductPricelistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPricelists_CardTypes_CardTypeId",
                table: "ProductPricelists");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_CardCards_CardId",
                table: "SaleOrders");

            migrationBuilder.DropTable(
                name: "CardCards");

            migrationBuilder.DropTable(
                name: "ResPartnerBanks");

            migrationBuilder.DropTable(
                name: "SaleSettings");

            migrationBuilder.DropTable(
                name: "CardTypes");

            migrationBuilder.DropTable(
                name: "ResBanks");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_CardId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProductPricelists_CardTypeId",
                table: "ProductPricelists");

            migrationBuilder.DropIndex(
                name: "IX_ProductPricelistItems_CardTypeId",
                table: "ProductPricelistItems");

            migrationBuilder.DropIndex(
                name: "IX_AccountJournals_BankAccountId",
                table: "AccountJournals");

            migrationBuilder.DropColumn(
                name: "CardId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "CardTypeId",
                table: "ProductPricelists");

            migrationBuilder.DropColumn(
                name: "CardTypeId",
                table: "ProductPricelistItems");

            migrationBuilder.DropColumn(
                name: "FixedAmountPrice",
                table: "ProductPricelistItems");

            migrationBuilder.DropColumn(
                name: "BankAccountId",
                table: "AccountJournals");
        }
    }
}
