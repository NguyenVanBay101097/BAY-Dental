using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSms_11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCareAfterOrderAutomation",
                table: "SmsConfigs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsThanksCustomerAutomation",
                table: "SmsConfigs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCareAfterOrderAutomation",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "IsThanksCustomerAutomation",
                table: "SmsConfigs");
        }
    }
}
