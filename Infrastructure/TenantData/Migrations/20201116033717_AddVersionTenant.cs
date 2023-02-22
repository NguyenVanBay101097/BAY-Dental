using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.TenantData.Migrations
{
    public partial class AddVersionTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Tenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Tenants");
        }
    }
}
