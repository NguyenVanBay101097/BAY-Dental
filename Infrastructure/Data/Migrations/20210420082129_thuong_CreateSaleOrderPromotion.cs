using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_CreateSaleOrderPromotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountDiscountTotal",
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
                    SaleCouponProgramId = table.Column<Guid>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: true),
                    SaleOrderLineId = table.Column<Guid>(nullable: true),
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
                        name: "FK_SaleOrderPromotions_SaleOrderPromotions_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SaleOrderPromotions",
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

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_CreatedById",
                table: "SaleOrderPromotions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderPromotions_ParentId",
                table: "SaleOrderPromotions",
                column: "ParentId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderPromotions");

            migrationBuilder.DropColumn(
                name: "AmountDiscountTotal",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "ToothType",
                table: "SaleOrderLines");
        }
    }
}
