using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class edit_respartnerbank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountHolderName",
                table: "ResPartnerBanks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "ResPartnerBanks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountHolderName",
                table: "ResPartnerBanks");

            migrationBuilder.DropColumn(
                name: "Branch",
                table: "ResPartnerBanks");
        }
    }
}
