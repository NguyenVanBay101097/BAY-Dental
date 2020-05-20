using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateConnectPageColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "FacebookUserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "FacebookPages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "FacebookPages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "FacebookUserProfiles");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "FacebookPages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "FacebookPages");
        }
    }
}
