using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F44 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "AccountInvoices",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountFixed",
                table: "AccountInvoices",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "AccountInvoices",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "AccountInvoices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "AccountInvoices");

            migrationBuilder.DropColumn(
                name: "DiscountFixed",
                table: "AccountInvoices");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "AccountInvoices");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "AccountInvoices");
        }
    }
}
