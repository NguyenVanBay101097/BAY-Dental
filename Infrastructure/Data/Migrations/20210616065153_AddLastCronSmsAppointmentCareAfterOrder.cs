using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddLastCronSmsAppointmentCareAfterOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastCron",
                table: "SmsCareAfterOrderAutomationConfigs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCron",
                table: "SmsAppointmentAutomationConfigs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCron",
                table: "SmsCareAfterOrderAutomationConfigs");

            migrationBuilder.DropColumn(
                name: "LastCron",
                table: "SmsAppointmentAutomationConfigs");
        }
    }
}
