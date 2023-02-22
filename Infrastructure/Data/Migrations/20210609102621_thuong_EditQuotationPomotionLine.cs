using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_EditQuotationPomotionLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationPromotionLines_QuotationLines_QuotationLineId",
                table: "QuotationPromotionLines");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuotationLineId",
                table: "QuotationPromotionLines",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationPromotionLines_QuotationLines_QuotationLineId",
                table: "QuotationPromotionLines",
                column: "QuotationLineId",
                principalTable: "QuotationLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationPromotionLines_QuotationLines_QuotationLineId",
                table: "QuotationPromotionLines");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuotationLineId",
                table: "QuotationPromotionLines",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationPromotionLines_QuotationLines_QuotationLineId",
                table: "QuotationPromotionLines",
                column: "QuotationLineId",
                principalTable: "QuotationLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
