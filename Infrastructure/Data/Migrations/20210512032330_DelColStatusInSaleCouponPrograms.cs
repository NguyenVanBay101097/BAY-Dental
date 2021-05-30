using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class DelColStatusInSaleCouponPrograms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SaleCouponPrograms");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "SaleCouponPrograms",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "SaleCouponPrograms");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SaleCouponPrograms",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
