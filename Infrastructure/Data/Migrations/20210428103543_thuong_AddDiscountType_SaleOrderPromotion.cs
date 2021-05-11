using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddDiscountType_SaleOrderPromotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountFixed",
                table: "SaleOrderPromotions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "SaleOrderPromotions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "SaleOrderPromotions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountFixed",
                table: "SaleOrderPromotions");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "SaleOrderPromotions");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "SaleOrderPromotions");
        }
    }
}
