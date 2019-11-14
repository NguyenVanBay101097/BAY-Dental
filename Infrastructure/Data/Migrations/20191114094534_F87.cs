using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F87 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Cost",
                table: "StockQuants",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "PriceUnit",
                table: "StockMoves",
                nullable: true,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "StockQuants",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceUnit",
                table: "StockMoves",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
