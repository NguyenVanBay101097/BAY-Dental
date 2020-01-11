using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F115 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CodePromoProgramId",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRewardLine",
                table: "SaleOrderLines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "RuleMinQuantity",
                table: "SaleCouponPrograms",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountApplyOn",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumUseNumber",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoCodeUsage",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RewardDescription",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RuleDateFrom",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RuleDateTo",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "SaleCouponPrograms",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_CodePromoProgramId",
                table: "SaleOrders",
                column: "CodePromoProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponProgramProductRels_ProductId",
                table: "SaleCouponProgramProductRels",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderNoCodePromoPrograms_ProgramId",
                table: "SaleOrderNoCodePromoPrograms",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_SaleCouponPrograms_CodePromoProgramId",
                table: "SaleOrders",
                column: "CodePromoProgramId",
                principalTable: "SaleCouponPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_SaleCouponPrograms_CodePromoProgramId",
                table: "SaleOrders");

            migrationBuilder.DropTable(
                name: "SaleCouponProgramProductRels");

            migrationBuilder.DropTable(
                name: "SaleOrderNoCodePromoPrograms");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_CodePromoProgramId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "CodePromoProgramId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "IsRewardLine",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "DiscountApplyOn",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "MaximumUseNumber",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "PromoCodeUsage",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "RewardDescription",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "RuleDateFrom",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "RuleDateTo",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "SaleCouponPrograms");

            migrationBuilder.AlterColumn<decimal>(
                name: "RuleMinQuantity",
                table: "SaleCouponPrograms",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
