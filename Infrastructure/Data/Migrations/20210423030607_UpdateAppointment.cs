using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateAppointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAppointmentReminder",
                table: "Appointments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeAppointment",
                table: "Appointments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAppointmentReminder",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DateTimeAppointment",
                table: "Appointments");
        }
    }
}
