using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_editCustomerReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateWaitting",
                table: "CustomerReceipts");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateWaiting",
                table: "CustomerReceipts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateWaiting",
                table: "CustomerReceipts");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateWaitting",
                table: "CustomerReceipts",
                type: "datetime2",
                nullable: true);
        }
    }
}
