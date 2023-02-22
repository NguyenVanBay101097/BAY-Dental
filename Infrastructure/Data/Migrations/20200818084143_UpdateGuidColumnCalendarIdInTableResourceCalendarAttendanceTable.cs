using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateGuidColumnCalendarIdInTableResourceCalendarAttendanceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceCalendarAttendances_ResourceCalendars_CalendarId",
                table: "ResourceCalendarAttendances");

            migrationBuilder.AlterColumn<Guid>(
                name: "CalendarId",
                table: "ResourceCalendarAttendances",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceCalendarAttendances_ResourceCalendars_CalendarId",
                table: "ResourceCalendarAttendances",
                column: "CalendarId",
                principalTable: "ResourceCalendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceCalendarAttendances_ResourceCalendars_CalendarId",
                table: "ResourceCalendarAttendances");

            migrationBuilder.AlterColumn<Guid>(
                name: "CalendarId",
                table: "ResourceCalendarAttendances",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceCalendarAttendances_ResourceCalendars_CalendarId",
                table: "ResourceCalendarAttendances",
                column: "CalendarId",
                principalTable: "ResourceCalendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
