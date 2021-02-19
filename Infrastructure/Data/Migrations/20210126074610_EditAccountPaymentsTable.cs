using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class EditAccountPaymentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DestinationAccountId",
                table: "AccountPayments",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LoaiThuChiId",
                table: "AccountPayments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayments_DestinationAccountId",
                table: "AccountPayments",
                column: "DestinationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayments_LoaiThuChiId",
                table: "AccountPayments",
                column: "LoaiThuChiId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPayments_AccountAccounts_DestinationAccountId",
                table: "AccountPayments",
                column: "DestinationAccountId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPayments_LoaiThuChis_LoaiThuChiId",
                table: "AccountPayments",
                column: "LoaiThuChiId",
                principalTable: "LoaiThuChis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountPayments_AccountAccounts_DestinationAccountId",
                table: "AccountPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountPayments_LoaiThuChis_LoaiThuChiId",
                table: "AccountPayments");

            migrationBuilder.DropIndex(
                name: "IX_AccountPayments_DestinationAccountId",
                table: "AccountPayments");

            migrationBuilder.DropIndex(
                name: "IX_AccountPayments_LoaiThuChiId",
                table: "AccountPayments");

            migrationBuilder.DropColumn(
                name: "DestinationAccountId",
                table: "AccountPayments");

            migrationBuilder.DropColumn(
                name: "LoaiThuChiId",
                table: "AccountPayments");
        }
    }
}
