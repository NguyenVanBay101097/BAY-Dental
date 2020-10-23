using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateTCareTablesV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AutoCustomType",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomDay",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomHour",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomMinute",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomMonth",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TCareScenarios",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoCustomType",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "CustomDay",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "CustomHour",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "CustomMinute",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "CustomMonth",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TCareScenarios");
        }
    }
}
