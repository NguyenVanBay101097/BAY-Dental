using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountOfTimes",
                table: "ToaThuocLines",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDays",
                table: "ToaThuocLines",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTimes",
                table: "ToaThuocLines",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UseAt",
                table: "ToaThuocLines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountOfTimes",
                table: "ToaThuocLines");

            migrationBuilder.DropColumn(
                name: "NumberOfDays",
                table: "ToaThuocLines");

            migrationBuilder.DropColumn(
                name: "NumberOfTimes",
                table: "ToaThuocLines");

            migrationBuilder.DropColumn(
                name: "UseAt",
                table: "ToaThuocLines");
        }
    }
}
