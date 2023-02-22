using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class SaleOrderLineDiscountFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountFixed",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "SaleOrderLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountFixed",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "SaleOrderLines");
        }
    }
}
