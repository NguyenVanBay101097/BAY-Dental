using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class EditResourceCalendarAttendancesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceCalendarAttendances_ResourceCalendars_CalendarId",
                table: "ResourceCalendarAttendances");

            migrationBuilder.AlterColumn<Guid>(
                name: "CalendarId",
                table: "ResourceCalendarAttendances",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceCalendarAttendances_ResourceCalendars_CalendarId",
                table: "ResourceCalendarAttendances",
                column: "CalendarId",
                principalTable: "ResourceCalendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceCalendarAttendances_ResourceCalendars_CalendarId",
                table: "ResourceCalendarAttendances",
                column: "CalendarId",
                principalTable: "ResourceCalendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
