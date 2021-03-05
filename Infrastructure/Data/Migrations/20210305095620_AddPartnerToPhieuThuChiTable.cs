using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddPartnerToPhieuThuChiTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "PhieuThuChis",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerType",
                table: "PhieuThuChis",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuChis_PartnerId",
                table: "PhieuThuChis",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThuChis_Partners_PartnerId",
                table: "PhieuThuChis",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThuChis_Partners_PartnerId",
                table: "PhieuThuChis");

            migrationBuilder.DropIndex(
                name: "IX_PhieuThuChis_PartnerId",
                table: "PhieuThuChis");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "PhieuThuChis");

            migrationBuilder.DropColumn(
                name: "PartnerType",
                table: "PhieuThuChis");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationAccountId",
                table: "AccountPayments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LoaiThuChiId",
                table: "AccountPayments",
                type: "uniqueidentifier",
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
    }
}
