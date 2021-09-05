using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class addDate_orderLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDone",
                table: "SaleOrderLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "DateDone",
                table: "SaleOrderLines");
        }
    }
}
