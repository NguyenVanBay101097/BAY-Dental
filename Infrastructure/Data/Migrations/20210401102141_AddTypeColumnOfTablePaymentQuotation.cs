using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTypeColumnOfTablePaymentQuotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "PaymentQuotations");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PaymentQuotations",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "DiscountPercentType",
                table: "PaymentQuotations",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Payment",
                table: "PaymentQuotations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercentType",
                table: "PaymentQuotations");

            migrationBuilder.DropColumn(
                name: "Payment",
                table: "PaymentQuotations");

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "PaymentQuotations",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercent",
                table: "PaymentQuotations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
