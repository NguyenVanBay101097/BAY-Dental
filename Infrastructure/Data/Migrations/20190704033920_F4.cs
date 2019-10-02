using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountTotalCompanySigned",
                table: "AccountInvoices");

            migrationBuilder.DropColumn(
                name: "AmountUntaxedSigned",
                table: "AccountInvoices");

            migrationBuilder.DropColumn(
                name: "ResidualCompanySigned",
                table: "AccountInvoices");

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodLockDate",
                table: "Companies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodLockDate",
                table: "Companies");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTotalCompanySigned",
                table: "AccountInvoices",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountUntaxedSigned",
                table: "AccountInvoices",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ResidualCompanySigned",
                table: "AccountInvoices",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
