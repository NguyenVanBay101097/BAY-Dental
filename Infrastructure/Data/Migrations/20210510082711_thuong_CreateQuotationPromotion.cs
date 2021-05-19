﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_CreateQuotationPromotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_Employees_AdvisoryEmployeeId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_AdvisoryEmployeeId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "AdvisoryEmployeeId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "QuotationLines");

            migrationBuilder.AddColumn<double>(
                name: "AmountDiscountTotal",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CounselorId",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmountFixed",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmountPercent",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "QuotationLines",
                nullable: true);

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
                name: "IX_QuotationLines_AssistantId",
                table: "QuotationLines",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_CounselorId",
                table: "QuotationLines",
                column: "CounselorId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_EmployeeId",
                table: "QuotationLines",
                column: "EmployeeId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_Employees_AssistantId",
                table: "QuotationLines",
                column: "AssistantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_Employees_CounselorId",
                table: "QuotationLines",
                column: "CounselorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_Employees_EmployeeId",
                table: "QuotationLines",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_Employees_AssistantId",
                table: "QuotationLines");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_Employees_CounselorId",
                table: "QuotationLines");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_Employees_EmployeeId",
                table: "QuotationLines");

            migrationBuilder.DropTable(
                name: "QuotationPromotionLines");

            migrationBuilder.DropTable(
                name: "QuotationPromotions");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_AssistantId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_CounselorId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_EmployeeId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "AmountDiscountTotal",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "CounselorId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "DiscountAmountFixed",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "DiscountAmountPercent",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "QuotationLines");

            migrationBuilder.AddColumn<Guid>(
                name: "AdvisoryEmployeeId",
                table: "QuotationLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "QuotationLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_AdvisoryEmployeeId",
                table: "QuotationLines",
                column: "AdvisoryEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_Employees_AdvisoryEmployeeId",
                table: "QuotationLines",
                column: "AdvisoryEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
