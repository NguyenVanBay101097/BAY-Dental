using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountExpenseId",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AccountIncomeId",
                table: "Companies",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AccountExpenseId",
                table: "Companies",
                column: "AccountExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AccountIncomeId",
                table: "Companies",
                column: "AccountIncomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AccountAccounts_AccountExpenseId",
                table: "Companies",
                column: "AccountExpenseId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AccountAccounts_AccountIncomeId",
                table: "Companies",
                column: "AccountIncomeId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AccountAccounts_AccountExpenseId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AccountAccounts_AccountIncomeId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_AccountExpenseId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_AccountIncomeId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "AccountExpenseId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "AccountIncomeId",
                table: "Companies");
        }
    }
}
