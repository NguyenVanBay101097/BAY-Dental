using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddColumnIsFastJournal_SaleOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JournalId",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isFast",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_JournalId",
                table: "SaleOrders",
                column: "JournalId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_AccountJournals_JournalId",
                table: "SaleOrders",
                column: "JournalId",
                principalTable: "AccountJournals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_AccountJournals_JournalId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_JournalId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "JournalId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "isFast",
                table: "SaleOrders");
        }
    }
}
