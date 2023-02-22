using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.TenantData.Migrations
{
    public partial class Bay_addNbCompanyactive_toTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveCompaniesNbr",
                table: "Tenants",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveCompaniesNbr",
                table: "Tenants");
        }
    }
}
