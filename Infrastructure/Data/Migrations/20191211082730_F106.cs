using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F106 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GroupLoyaltyCard",
                table: "ResConfigSettings",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GroupSaleCouponPromotion",
                table: "ResConfigSettings",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LoyaltyPointExchangeRate",
                table: "ResConfigSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupLoyaltyCard",
                table: "ResConfigSettings");

            migrationBuilder.DropColumn(
                name: "GroupSaleCouponPromotion",
                table: "ResConfigSettings");

            migrationBuilder.DropColumn(
                name: "LoyaltyPointExchangeRate",
                table: "ResConfigSettings");
        }
    }
}
