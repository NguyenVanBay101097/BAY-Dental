using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F72 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type2",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ProductCategories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type2",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductCategories");
        }
    }
}
