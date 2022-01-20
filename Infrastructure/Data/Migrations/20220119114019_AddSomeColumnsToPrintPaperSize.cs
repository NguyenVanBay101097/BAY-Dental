using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddSomeColumnsToPrintPaperSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HeaderLine",
                table: "PrintPaperSizes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HeaderSpacing",
                table: "PrintPaperSizes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Orientation",
                table: "PrintPaperSizes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderLine",
                table: "PrintPaperSizes");

            migrationBuilder.DropColumn(
                name: "HeaderSpacing",
                table: "PrintPaperSizes");

            migrationBuilder.DropColumn(
                name: "Orientation",
                table: "PrintPaperSizes");
        }
    }
}
