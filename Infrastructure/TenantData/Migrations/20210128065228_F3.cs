using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.TenantData.Migrations
{
    public partial class F3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerSource",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupporterName",
                table: "Tenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerSource",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SupporterName",
                table: "Tenants");
        }
    }
}
