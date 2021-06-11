using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Thang_Update_Sms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeSend",
                table: "SmsMessages");

            migrationBuilder.AddColumn<int>(
                name: "ResCount",
                table: "SmsMessages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDate",
                table: "SmsMessages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SmsAccounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResCount",
                table: "SmsMessages");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                table: "SmsMessages");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SmsAccounts");

            migrationBuilder.AddColumn<string>(
                name: "TypeSend",
                table: "SmsMessages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
