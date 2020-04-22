using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateServiceCardOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountTotal",
                table: "ServiceCardOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GenerationType",
                table: "ServiceCardOrders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceUnit",
                table: "ServiceCardOrders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "ServiceCardOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountTotal",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "GenerationType",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "PriceUnit",
                table: "ServiceCardOrders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ServiceCardOrders");
        }
    }
}
