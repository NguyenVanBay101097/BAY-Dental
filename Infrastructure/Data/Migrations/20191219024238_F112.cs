using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F112 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CompanySharePartner",
                table: "ResConfigSettings",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CompanyShareProduct",
                table: "ResConfigSettings",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GroupMultiCompany",
                table: "ResConfigSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanySharePartner",
                table: "ResConfigSettings");

            migrationBuilder.DropColumn(
                name: "CompanyShareProduct",
                table: "ResConfigSettings");

            migrationBuilder.DropColumn(
                name: "GroupMultiCompany",
                table: "ResConfigSettings");
        }
    }
}
