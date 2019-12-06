using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F97 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CouponId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PromotionProgramId",
                table: "SaleOrderLines",
                nullable: true);

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
                    RuleMinimumAmount = table.Column<decimal>(nullable: true),
                    RuleMinQuantity = table.Column<decimal>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    DiscountType = table.Column<string>(nullable: true),
                    DiscountPercentage = table.Column<decimal>(nullable: true),
                    DiscountFixedAmount = table.Column<decimal>(nullable: true),
                    DiscountLineProductId = table.Column<Guid>(nullable: true),
                    ValidityDuration = table.Column<int>(nullable: true),
                    RewardType = table.Column<string>(nullable: true),
                    ProgramType = table.Column<string>(nullable: true),
                    PromoApplicability = table.Column<string>(nullable: true),
                    DiscountMaxAmount = table.Column<decimal>(nullable: true)
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
                        name: "FK_SaleCouponPrograms_AspNetUsers_WriteById",
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
                    OrderId = table.Column<Guid>(nullable: true),
                    ProgramType = table.Column<string>(nullable: true)
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
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_CouponId",
                table: "SaleOrderLines",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_PromotionProgramId",
                table: "SaleOrderLines",
                column: "PromotionProgramId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_SaleCoupons_CouponId",
                table: "SaleOrderLines",
                column: "CouponId",
                principalTable: "SaleCoupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_SaleCouponPrograms_PromotionProgramId",
                table: "SaleOrderLines",
                column: "PromotionProgramId",
                principalTable: "SaleCouponPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_SaleCoupons_CouponId",
                table: "SaleOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_SaleCouponPrograms_PromotionProgramId",
                table: "SaleOrderLines");

            migrationBuilder.DropTable(
                name: "SaleCoupons");

            migrationBuilder.DropTable(
                name: "SaleCouponPrograms");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_CouponId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_PromotionProgramId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "PromotionProgramId",
                table: "SaleOrderLines");
        }
    }
}
