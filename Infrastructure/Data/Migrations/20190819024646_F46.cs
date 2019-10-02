using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F46 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LaboCateg",
                table: "ProductCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MedicineCateg",
                table: "ProductCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProductCateg",
                table: "ProductCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ServiceCateg",
                table: "ProductCategories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LaboCateg",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "MedicineCateg",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ProductCateg",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ServiceCateg",
                table: "ProductCategories");
        }
    }
}
