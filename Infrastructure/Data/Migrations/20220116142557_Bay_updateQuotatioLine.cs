using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_updateQuotatioLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_ToothCategories_ToothCategoryId",
                table: "QuotationLines");

            migrationBuilder.AlterColumn<Guid>(
                name: "ToothCategoryId",
                table: "QuotationLines",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_ToothCategories_ToothCategoryId",
                table: "QuotationLines",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_ToothCategories_ToothCategoryId",
                table: "QuotationLines");

            migrationBuilder.AlterColumn<Guid>(
                name: "ToothCategoryId",
                table: "QuotationLines",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_ToothCategories_ToothCategoryId",
                table: "QuotationLines",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
