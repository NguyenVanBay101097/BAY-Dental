using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_Update_toathuoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceStatus",
                table: "ToaThuocs",
                nullable: true, defaultValue: "to_invoice");

            migrationBuilder.AddColumn<decimal>(
                name: "ToInvoiceQuantity",
                table: "ToaThuocLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceStatus",
                table: "ToaThuocs");

            migrationBuilder.DropColumn(
                name: "ToInvoiceQuantity",
                table: "ToaThuocLines");
        }
    }
}
