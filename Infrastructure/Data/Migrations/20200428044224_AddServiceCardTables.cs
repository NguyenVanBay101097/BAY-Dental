using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddServiceCardTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GroupServiceCard",
                table: "ResConfigSettings",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceCardTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    Period = table.Column<string>(nullable: true),
                    NbrPeriod = table.Column<int>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCardTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCardTypes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardTypes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardTypes_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCardOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false),
                    CardTypeId = table.Column<Guid>(nullable: false),
                    DateOrder = table.Column<DateTime>(nullable: false),
                    ActivatedDate = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    MoveId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    PriceUnit = table.Column<decimal>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: true),
                    AmountTotal = table.Column<decimal>(nullable: true),
                    GenerationType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCardOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrders_ServiceCardTypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "ServiceCardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrders_AccountMoves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "AccountMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrders_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrders_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCardCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CardTypeId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: true),
                    ActivatedDate = table.Column<DateTime>(nullable: true),
                    ExpiredDate = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    Residual = table.Column<decimal>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    OrderId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCardCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCardCards_ServiceCardTypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "ServiceCardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardCards_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardCards_ServiceCardOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ServiceCardOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardCards_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardCards_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCardOrderPartnerRels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CardOrderId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCardOrderPartnerRels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPartnerRels_ServiceCardOrders_CardOrderId",
                        column: x => x.CardOrderId,
                        principalTable: "ServiceCardOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPartnerRels_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPartnerRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCardOrderPartnerRels_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderServiceCardCardRels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: false),
                    CardId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderServiceCardCardRels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderServiceCardCardRels_ServiceCardCards_CardId",
                        column: x => x.CardId,
                        principalTable: "ServiceCardCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderServiceCardCardRels_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderServiceCardCardRels_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderServiceCardCardRels_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderServiceCardCardRels_CardId",
                table: "SaleOrderServiceCardCardRels",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderServiceCardCardRels_CreatedById",
                table: "SaleOrderServiceCardCardRels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderServiceCardCardRels_SaleOrderId",
                table: "SaleOrderServiceCardCardRels",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderServiceCardCardRels_WriteById",
                table: "SaleOrderServiceCardCardRels",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardCards_CardTypeId",
                table: "ServiceCardCards",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardCards_CreatedById",
                table: "ServiceCardCards",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardCards_OrderId",
                table: "ServiceCardCards",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardCards_PartnerId",
                table: "ServiceCardCards",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardCards_WriteById",
                table: "ServiceCardCards",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPartnerRels_CardOrderId",
                table: "ServiceCardOrderPartnerRels",
                column: "CardOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPartnerRels_CreatedById",
                table: "ServiceCardOrderPartnerRels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPartnerRels_PartnerId",
                table: "ServiceCardOrderPartnerRels",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrderPartnerRels_WriteById",
                table: "ServiceCardOrderPartnerRels",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_CardTypeId",
                table: "ServiceCardOrders",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_CompanyId",
                table: "ServiceCardOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_CreatedById",
                table: "ServiceCardOrders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_MoveId",
                table: "ServiceCardOrders",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_PartnerId",
                table: "ServiceCardOrders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_UserId",
                table: "ServiceCardOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardOrders_WriteById",
                table: "ServiceCardOrders",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardTypes_CompanyId",
                table: "ServiceCardTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardTypes_CreatedById",
                table: "ServiceCardTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardTypes_ProductId",
                table: "ServiceCardTypes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardTypes_WriteById",
                table: "ServiceCardTypes",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderServiceCardCardRels");

            migrationBuilder.DropTable(
                name: "ServiceCardOrderPartnerRels");

            migrationBuilder.DropTable(
                name: "ServiceCardCards");

            migrationBuilder.DropTable(
                name: "ServiceCardOrders");

            migrationBuilder.DropTable(
                name: "ServiceCardTypes");

            migrationBuilder.DropColumn(
                name: "GroupServiceCard",
                table: "ResConfigSettings");
        }
    }
}
