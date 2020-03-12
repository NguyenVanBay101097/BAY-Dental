using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateMarketingTraceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Delivery",
                table: "MarketingTraces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageId",
                table: "MarketingTraces",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Read",
                table: "MarketingTraces",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Delivery",
                table: "MarketingTraces");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "MarketingTraces");

            migrationBuilder.DropColumn(
                name: "Read",
                table: "MarketingTraces");
        }
    }
}
