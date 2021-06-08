using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddRefCustomerInPhieuThuChi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "PhieuThuChis",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuChis_CustomerId",
                table: "PhieuThuChis",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThuChis_Partners_CustomerId",
                table: "PhieuThuChis",
                column: "CustomerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThuChis_Partners_CustomerId",
                table: "PhieuThuChis");

            migrationBuilder.DropIndex(
                name: "IX_PhieuThuChis_CustomerId",
                table: "PhieuThuChis");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "PhieuThuChis");
        }
    }
}
