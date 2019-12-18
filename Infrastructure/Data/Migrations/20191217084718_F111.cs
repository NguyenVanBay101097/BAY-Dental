using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountInvoiced",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountToInvoice",
                table: "SaleOrderLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountInvoiced",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "AmountToInvoice",
                table: "SaleOrderLines");
        }
    }
}
