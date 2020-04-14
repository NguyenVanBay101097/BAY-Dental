using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddAccountMoveLineDiscountFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountFixed",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "AccountMoveLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountFixed",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "AccountMoveLines");
        }
    }
}
