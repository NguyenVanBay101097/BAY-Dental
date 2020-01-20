using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F120 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoSendBirthdayMessage",
                table: "ZaloOAConfigs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BirthdayMessageContent",
                table: "ZaloOAConfigs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoSendBirthdayMessage",
                table: "ZaloOAConfigs");

            migrationBuilder.DropColumn(
                name: "BirthdayMessageContent",
                table: "ZaloOAConfigs");
        }
    }
}
